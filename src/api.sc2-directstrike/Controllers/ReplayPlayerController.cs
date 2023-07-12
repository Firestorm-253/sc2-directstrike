using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;
using System.Numerics;

[ApiController]
[Route("{pkt}/" + NAME)]
public class ReplayPlayerController : ControllerBase
{
    public const string NAME = "replay_players";

    [HttpGet("{id}")]
    public async Task<ReplayPlayer?> GetById(string pkt, int id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "Id", id);

        var result = await Program.DbContext.ReadFromDb(query);
        var entry = result.SingleOrDefault();

        if (entry == null)
        {
            return null;
        }
        return entry;
    }

    [HttpGet]
    public async Task<IEnumerable<ReplayPlayer>> Get(string pkt,
                                                      [FromQuery(Name = "replayId")] int? replayId = null,
                                                      [FromQuery(Name = "playerId")] int? playerId = null)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "ReplayId", replayId);
        query += DbContext.AddCondition(query, "PlayerId", playerId);

        var result = await Program.DbContext.ReadFromDb(query);

        return result.Select(entry => (ReplayPlayer)entry!);
    }


    public static async Task<ReplayPlayer> GenerateIncrementedReplay(string pkt, PostReplayPlayer postReplayPlayer, Replay replay, Player player)
    {
        ReplayPlayer replayPlayer = ((ReplayPlayer)postReplayPlayer) with
        {
            ReplayId = replay.Id,
            PlayerId = player.Id,
        };
        await Program.DbContext.WriteToDb(pkt, NAME, replayPlayer);
        
        var result = await Program.DbContext.ReadFromDb($"SELECT Id FROM {NAME} WHERE PKT='{pkt}'");
        return replayPlayer with { Id = (uint)result.Last()["Id"], };
    }
}
