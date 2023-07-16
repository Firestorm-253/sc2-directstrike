namespace sc2_directstrike.api.Services;
using DTOs;
using Contexts;
using Data.RatingService.ProcessData;
using Data.RatingService;
using System.Text;
using System.Linq;
using sc2_directstrike.api.Controllers;

public partial class RatingService
{
    private readonly IServiceProvider serviceProvider;

    private Dictionary<ulong, Replay> replays = null!; // [ReplayId] -> replay
    private Dictionary<ulong, ReplayPlayer> replayPlayers = null!; // [ReplayPlayerId] -> replayPlayer
    private Dictionary<ulong, List<ReplayPlayer>> replays_replayPlayers = null!; // [ReplayId] -> replayPlayers
    private Dictionary<string, Dictionary<ulong, List<ReplayPlayerRating>>> playerRatings = null!; // [GameMode][PlayerId] -> replayPlayerRatings

    public RatingService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    private async Task LoadFromDb(string pkt)
    {
        using var scope = this.serviceProvider.CreateScope();

        var replayContext = scope.ServiceProvider.GetRequiredService<ReplayContext>();
        var replayPlayerContext = scope.ServiceProvider.GetRequiredService<ReplayPlayerContext>();
        //var playerContext = scope.ServiceProvider.GetRequiredService<PlayerContext>();

        this.replays = (await replayContext.Get(pkt, Array.Empty<string>(), "*")).OrderBy(r => r.GameTime).ToDictionary(r => r.Id);
        this.replayPlayers = (await replayPlayerContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(rp => rp.Id);
        //var players = (await playerContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(p => p.Id);

        this.playerRatings = new();
        this.replays_replayPlayers = new();

        foreach (var ent_replayPlayer in replayPlayers)
        {
            var replayPlayer = ent_replayPlayer.Value;
            var replay = replays[replayPlayer.ReplayId];

            if (!this.playerRatings.ContainsKey(replay.GameMode))
            {
                this.playerRatings.Add(replay.GameMode, new());
            }
            if (!this.playerRatings[replay.GameMode].ContainsKey(replayPlayer.PlayerId))
            {
                this.playerRatings[replay.GameMode].Add(replayPlayer.PlayerId, new());
            }

            if (!this.replays_replayPlayers.ContainsKey(replayPlayer.ReplayId))
            {
                this.replays_replayPlayers.Add(replayPlayer.ReplayId, new());
            }
            this.replays_replayPlayers[replayPlayer.ReplayId].Add(replayPlayer);
        }
    }

    private async Task UpdateRatings(string pkt, bool insert = true)
    {
        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        var pktId = (await dbContext.ReadFromDb(PKTController.GetQuery(pkt)))[0]["Id"];

        await dbContext.WriteToDb($"DELETE FROM ratings WHERE PKT = '{pktId}' ");

        var playerRatings = this.playerRatings.SelectMany(x => x.Value.SelectMany(y => y.Value));

        int chunck_size = 1_000;
        for (int i = 0; i < playerRatings.Count(); i += chunck_size)
        {
            var allValues = new StringBuilder();

            for (int k = 0; k < chunck_size; k++)
            {
                if (i + k >= playerRatings.Count())
                {
                    break;
                }

                var replayPlayerRating = playerRatings.ElementAt(i + k);

                var values = new StringBuilder();
                if (k > 0)
                {
                    values.Append(',');
                }

                values.Append($"('{pktId}',");
                values.Append($"'{replayPlayerRating.ReplayPlayerId}',");
                values.Append($"'{replayPlayerRating.RatingBefore}',");
                values.Append($"'{replayPlayerRating.RatingAfter}',");
                values.Append($"'{replayPlayerRating.DeviationBefore}',");
                values.Append($"'{replayPlayerRating.DeviationAfter}')");

                allValues.Append(values);
            }

            string query =
                $"INSERT INTO ratings(PKT,ReplayPlayerId,RatingBefore,RatingAfter,DeviationBefore,DeviationAfter) " +
                $"VALUES {allValues} ";

            await dbContext.WriteToDb(query);
        }
    }

    public async Task ReCalc(string pkt)
    {
        await this.LoadFromDb(pkt);

        foreach (var ent_replay in this.replays)
        {
            var replay = ent_replay.Value;
            var replayData = new ReplayData(replay, replays_replayPlayers[replay.Id]);

            if (replayData.Team1.Players.Length != replayData.Team2.Players.Length)
            {
                continue;
                //throw new Exception("ERROR: Unequal Teams!");
            }
            if (!replayData.Team1.IsWinner && !replayData.Team2.IsWinner)
            {
                continue;
                //throw new Exception("ERROR: No Winner!");
            }

            this.ProcessReplay(replayData, RatingOptions.Default);
        }

        await UpdateRatings(pkt);
    }

    private void ProcessReplay(ReplayData replayData, RatingOptions ratingOptions)
    {
        SetReplayData(replayData, ratingOptions);

        CalculateRatingsDeltas(replayData.Team1, replayData, ratingOptions);
        CalculateRatingsDeltas(replayData.Team2, replayData, ratingOptions);
    }
}
