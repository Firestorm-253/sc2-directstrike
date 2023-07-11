using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("{pkt}/" + NAME)]
public class ReplayPlayerController : ControllerBase
{
    public const string NAME = "replay_players";

    [HttpGet("{id}")]
    public async Task<ReplayPlayer?> GetById(string pkt, int id)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "Id", id);

        var result = await Program.DbContext.ReadFromDb(query);
        var entry = result.Single();

        return entry;
    }

    [HttpGet]
    public async Task<IEnumerable<ReplayPlayer?>> Get(string pkt,
                                                      [FromQuery(Name = "replayId")] int? replayId,
                                                      [FromQuery(Name = "playerId")] int? playerId)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "ReplayId", replayId);
        query += DbContext.AddCondition(query, "PlayerId", playerId);

        var result = await Program.DbContext.ReadFromDb(query);

        return result.Select(entry => (ReplayPlayer)entry!);
    }


    public static async Task<ReplayPlayer> GenerateIncrementedReplay(string pkt, PostReplayPlayer postReplayPlayer)
    {
        ReplayPlayer replayPlayer = postReplayPlayer;
        await Program.DbContext.WriteToDb(pkt, NAME, replayPlayer);
        
        var result = await Program.DbContext.ReadFromDb($"SELECT Id FROM {NAME} WHERE PKT='{pkt}'");
        return replayPlayer with { Id = (uint)result.Last()["Id"], };
    }
}
