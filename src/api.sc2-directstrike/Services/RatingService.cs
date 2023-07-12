using System.Xml.Linq;

namespace api.sc2_directstrike.Services;
using DTOs;
using Controllers;
using api.sc2_directstrike.Contexts;

public class RatingService
{
    private readonly IServiceProvider serviceProvider;

    public RatingService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    private async Task<(Dictionary<int, Replay>, Dictionary<int, ReplayPlayer>, Dictionary<int, Player>)> LoadFromDb(string pkt)
    {
        using var scope = this.serviceProvider.CreateScope();

        var replayContext = scope.ServiceProvider.GetRequiredService<ReplayContext>();
        var replayPlayerContext = scope.ServiceProvider.GetRequiredService<ReplayPlayerContext>();
        var playerContext = scope.ServiceProvider.GetRequiredService<PlayerContext>();

        var replays = (await replayContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(r => (int)r.Id);
        var replayPlayers = (await replayPlayerContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(rp => (int)rp.Id);
        var players = (await playerContext.Get(pkt, Array.Empty<string>(), "*")).ToDictionary(p => (int)p.Id);

        return (replays, replayPlayers, players);
    }

    private async Task UpdateRatings(string pkt, Dictionary<int, ReplayPlayer> replayPlayers)
    {
        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        // ToDo Update all elements in 1 command?
        foreach (var ent in replayPlayers)
        {
            await dbContext.UpdateDb(pkt, ReplayPlayerController.NAME, ent.Value);
        }
    }

    public async Task ReCalc(string pkt)
    {
        var (replays, replayPlayers, players) = await LoadFromDb(pkt);

        // ToDo
        //replayPlayers[2].RatingAfter = 200;

        await UpdateRatings(pkt, replayPlayers);
    }
}
