using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;
using Contexts;

[ApiController]
[Route("{pkt}/" + NAME)]
public class ReplayController : ControllerBase
{
    public const string NAME = "replays";

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
    public async Task Post(string pkt, [FromBody] PostReplay postReplay)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        Replay replay = await GenerateIncrementedReplay(pkt, postReplay, dbContext);

        foreach (var postReplayPlayer in postReplay.ReplayPlayers)
        {
            Player player = await PlayerController.GenerateIncrementedPlayer(pkt, postReplayPlayer.Player, dbContext);
            ReplayPlayer replayPlayer = await ReplayPlayerController.GenerateIncrementedReplay(pkt, postReplayPlayer, replay, player, dbContext);
        }
    }


    public static async Task<Replay> GenerateIncrementedReplay(string pkt, PostReplay postReplay, DbContext dbContext)
    {
        Replay replay = postReplay;
        ulong id = await dbContext.WriteToDb(pkt, NAME, replay);

        return replay with { Id = id };
    }
}
