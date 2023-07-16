namespace sc2_directstrike.api.Services;
using DTOs;
using Contexts;
using Data.RatingService.ProcessData;
using Data.RatingService;

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

        // ToDo Update all elements in 1 command?
        foreach (var mode in this.playerRatings.Keys)
        {
            foreach (var playerId in this.playerRatings[mode].Keys)
            {
                var replayPlayer_ratings = this.playerRatings[mode][playerId];

                foreach (var replayPlayerRating in replayPlayer_ratings)
                {
                    if (insert)
                    {
                        await dbContext.WriteToDb(pkt, "ratings", replayPlayerRating);
                    }
                    else
                    {
                        await dbContext.UpdateDb(pkt, "ratings", replayPlayerRating);
                    }
                }
            }
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
                throw new Exception("ERROR: Unequal Teams!");
            }
            if (!replayData.Team1.IsWinner && !replayData.Team2.IsWinner)
            {
                throw new Exception("ERROR: No Winner!");
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
