using System.Xml.Linq;

namespace sc2_directstrike.api.Services;
using DTOs;
using Contexts;
using Data.RatingService.ProcessData;
using Data.RatingService;

public partial class RatingService
{
    private readonly IServiceProvider serviceProvider;

    private Dictionary<ulong, Replay> replays = null!;
    private Dictionary<ulong, List<ReplayPlayer>> replayPlayers = null!;
    private Dictionary<string, Dictionary<ulong, List<ReplayPlayer>>> playerRatings = null!;

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
        var replayPlayers = (await replayPlayerContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(rp => rp.Id);
        //var players = (await playerContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(p => p.Id);

        this.playerRatings = new();
        this.replayPlayers = new();

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
            //this.playerRatings[replay.GameMode][replayPlayer.PlayerId].Add(replayPlayer);

            if (!this.replayPlayers.ContainsKey(replayPlayer.ReplayId))
            {
                this.replayPlayers.Add(replayPlayer.ReplayId, new());
            }
            this.replayPlayers[replayPlayer.ReplayId].Add(replayPlayer);
        }
    }

    private async Task UpdateRatings(string pkt, IEnumerable<ReplayPlayer> replayPlayers)
    {
        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        // ToDo Update all elements in 1 command?
        foreach (var replayPlayer in replayPlayers)
        {
            await dbContext.UpdateDb(pkt, ReplayPlayerContext.Table, replayPlayer);
        }
    }

    public async Task ReCalc(string pkt)
    {
        await this.LoadFromDb(pkt);

        foreach (var ent_replay in this.replays)
        {
            var replay = ent_replay.Value;
            var replayData = new ReplayData(replay, replayPlayers[replay.Id]);

            if (replayData.Team1.Players.Length != 3 || replayData.Team2.Players.Length != 3)
            {
                throw new Exception();
            }
            if (!replayData.Team1.IsWinner && !replayData.Team2.IsWinner)
            {
                throw new Exception();
            }

            this.ProcessReplay(replayData, RatingOptions.Default);
        }

        await UpdateRatings(pkt, replayPlayers.SelectMany(re => re.Value));
    }

    private void ProcessReplay(ReplayData replayData, RatingOptions ratingOptions)
    {
        SetReplayData(replayData, ratingOptions);

        CalculateRatingsDeltas(replayData.Team1, replayData, ratingOptions);
        CalculateRatingsDeltas(replayData.Team2, replayData, ratingOptions);
    }
}
