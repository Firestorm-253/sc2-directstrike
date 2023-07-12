using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Xml.Linq;

namespace api.sc2_directstrike.Controllers;
using DTOs;
using Contexts;

[ApiController]
[Route("{pkt}/" + ReplayPlayerContext.Table)]
public class ReplayPlayerController : ControllerBase
{
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

    [HttpDelete]
    public async Task Delete(string pkt,
                            [FromQuery(Name = "replayId")] ulong? replayId = null,
                            [FromQuery(Name = "playerId")] ulong? playerId = null)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"DELETE " +
            $"FROM {ReplayPlayerContext.Table} " +
            $"WHERE PKT = '{pkt}' ";

        if (replayId != null)
        {
            query += $"AND ReplayId = '{replayId}' ";
        }
        if (playerId != null)
        {
            query += $"AND PlayerId = '{playerId}' ";
        }

        await dbContext.WriteToDb(query);
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


    public static async Task<ReplayPlayer> GenerateIncrementedReplay(string pkt, PostReplayPlayer postReplayPlayer, Replay replay, Player player, DbContext dbContext)
    {
        ReplayPlayer replayPlayer = ((ReplayPlayer)postReplayPlayer) with
        {
            ReplayId = replay.Id,
            PlayerId = player.Id,
        };
        ulong id = await dbContext.WriteToDb(pkt, ReplayPlayerContext.Table, replayPlayer);
        
        return replayPlayer with { Id = id };
    }
}
