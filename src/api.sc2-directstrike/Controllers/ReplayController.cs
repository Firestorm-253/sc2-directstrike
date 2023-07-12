using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;
using Contexts;

[ApiController]
[Route("{pkt}/" + ReplayContext.Table)]
public class ReplayController : ControllerBase
{
    private readonly IServiceProvider serviceProvider;

    public ReplayController(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [HttpGet]
    public async Task<IEnumerable<Replay>> Get(string pkt)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var replayContext = scope.ServiceProvider.GetRequiredService<ReplayContext>();

        return await replayContext.Get(pkt, conditions: Array.Empty<string>(), selects: "*");
    }

    [HttpGet("{id}")]
    public async Task<Replay?> GetById(string pkt, ulong id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = serviceProvider.CreateScope();
        var replayContext = scope.ServiceProvider.GetRequiredService<ReplayContext>();

        var result = await replayContext.Get(pkt, conditions: new string[] { $"Id = '{id}'" }, "*");
        var entry = result.SingleOrDefault();

        if (entry == null)
        {
            return null;
        }
        return entry;
    }

    [HttpPost]
    public async Task Post(string pkt, [FromBody] IEnumerable<PostReplay> postReplays)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        foreach (var postReplay in postReplays)
        {
            Replay replay = await GenerateIncrementedReplay(pkt, postReplay, dbContext);

            foreach (var postReplayPlayer in postReplay.ReplayPlayers)
            {
                Player player = await PlayerController.GenerateIncrementedPlayer(pkt, postReplayPlayer.Player, dbContext);
                ReplayPlayer replayPlayer = await ReplayPlayerController.GenerateIncrementedReplay(pkt, postReplayPlayer, replay, player, dbContext);
            }
        }
    }

    [HttpDelete]
    public async Task Delete(string pkt)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        await dbContext.WriteToDb($"DELETE FROM {ReplayContext.Table} ");
    }

    [HttpDelete("{id}")]
    public async Task Delete(string pkt, ulong id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        await dbContext.WriteToDb($"DELETE FROM {ReplayContext.Table} WHERE Id = '{id}' ");
    }


    public static async Task<Replay> GenerateIncrementedReplay(string pkt, PostReplay postReplay, DbContext dbContext)
    {
        Replay replay = postReplay;
        ulong id = await dbContext.WriteToDb(pkt, ReplayContext.Table, replay);

        return replay with { Id = id };
    }
}
