using System.Xml.Linq;

namespace sc2_directstrike.api.Services;
using DTOs;
using Contexts;

public class RatingService
{
    private readonly IServiceProvider serviceProvider;

    private Dictionary<ulong, Replay> replays = null!;
    private Dictionary<ulong, List<ReplayPlayer>> replays_replayPlayers = null!;

    public RatingService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    private async Task LoadFromDb(string pkt)
    {
        using var scope = this.serviceProvider.CreateScope();

        var replayContext = scope.ServiceProvider.GetRequiredService<ReplayContext>();
        var replayPlayerContext = scope.ServiceProvider.GetRequiredService<ReplayPlayerContext>();
        var playerContext = scope.ServiceProvider.GetRequiredService<PlayerContext>();

        this.replays = (await replayContext.Get(pkt, Array.Empty<string>(), "*")).OrderBy(r => r.GameTime).ToDictionary(r => r.Id);
        var replayPlayers = (await replayPlayerContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(rp => rp.Id);
        //var players = (await playerContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(p => p.Id);

        this.replays_replayPlayers = new Dictionary<ulong, List<ReplayPlayer>>();
        foreach (var ent_replayPlayer in replayPlayers)
        {
            var replayPlayer = ent_replayPlayer.Value;

            if (!replays_replayPlayers.ContainsKey(replayPlayer.ReplayId))
            {
                replays_replayPlayers.Add(replayPlayer.ReplayId, new List<ReplayPlayer>());
            }

            replays_replayPlayers[replayPlayer.ReplayId].Add(replayPlayer);
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
            this.ProcessReplay(ent_replay.Value);
        }

        await UpdateRatings(pkt, this.replays_replayPlayers.SelectMany(r => r.Value));
    }

    private void ProcessReplay(Replay replay)
    {
        var replayPlayers = this.replays_replayPlayers[replay.Id];

        // ToDo
        replayPlayers[0].RatingAfter = 12;
    }
}
