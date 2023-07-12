using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Xml.Linq;

namespace api.sc2_directstrike.Controllers;
using DTOs;
using Contexts;

[ApiController]
[Route("{pkt}/" + NAME)]
public class ReplayPlayerController : ControllerBase
{
    public const string NAME = "replay_players";
    
    private readonly IServiceProvider serviceProvider;

    public ReplayPlayerController(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [HttpGet("{id}")]
    public async Task<ReplayPlayer?> GetById(string pkt, ulong id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var replayPlayerContext = scope.ServiceProvider.GetRequiredService<ReplayPlayerContext>();

        var result = await replayPlayerContext.Get(pkt, conditions: new string[] { $"Id = '{id}'" }, "*");
        var entry = result.SingleOrDefault();

        if (entry == null)
        {
            return null;
        }
        return entry;
    }

    [HttpGet]
    public async Task<IEnumerable<ReplayPlayer>> Get(string pkt,
                                                     [FromQuery(Name = "replayId")] ulong? replayId = null,
                                                     [FromQuery(Name = "playerId")] ulong? playerId = null)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var replayPlayerContext = scope.ServiceProvider.GetRequiredService<ReplayPlayerContext>();

        var conditions = new List<string>();
        if (replayId != null)
        {
            conditions.Add($"ReplayId = '{replayId}'");
        }
        if (playerId != null)
        {
            conditions.Add($"PlayerId = '{playerId}'");
        }

        var result = await replayPlayerContext.Get(pkt, conditions, "*");

        return result.Select(entry => (ReplayPlayer)entry!);
    }


    public static async Task<ReplayPlayer> GenerateIncrementedReplay(string pkt, PostReplayPlayer postReplayPlayer, Replay replay, Player player, DbContext dbContext)
    {
        ReplayPlayer replayPlayer = ((ReplayPlayer)postReplayPlayer) with
        {
            ReplayId = replay.Id,
            PlayerId = player.Id,
        };
        ulong id = await dbContext.WriteToDb(pkt, NAME, replayPlayer);
        
        return replayPlayer with { Id = id };
    }
}
