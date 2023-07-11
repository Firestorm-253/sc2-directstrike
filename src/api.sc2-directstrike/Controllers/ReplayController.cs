using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("{pkt}/" + NAME)]
public class ReplayController : ControllerBase
{
    public const string NAME = "replays";

    [HttpGet]
    public async Task<IEnumerable<Replay?>> Get(string pkt)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);

        var result = await Program.DbContext.ReadFromDb(query);

        return result.Select(entry => (Replay)entry!);
    }

    [HttpGet("{id}")]
    public async Task<Replay?> GetById(string pkt, int id)
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

    [HttpPost]
    public async Task Post(string pkt, [FromBody] PostReplay postReplay)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        Replay replay = await GenerateIncrementedReplay(pkt, postReplay);
        var replayPlayers = new List<ReplayPlayer>();

        foreach (var postReplayPlayer in postReplay.ReplayPlayers)
        {
            Player player = await PlayerController.GenerateIncrementedPlayer(pkt, postReplayPlayer.Player);
            ReplayPlayer replayPlayer = await ReplayPlayerController.GenerateIncrementedReplay(pkt, postReplayPlayer);

            replayPlayer = replayPlayer with
            {
                ReplayId = replay.Id,
                PlayerId = player.Id,
            };
            await Program.DbContext.UpdateDb(pkt, ReplayPlayerController.NAME, replayPlayer);

            replayPlayers.Add(replayPlayer);
        }
    }

    public static async Task<Replay> GenerateIncrementedReplay(string pkt, PostReplay postReplay)
    {
        Replay replay = postReplay;
        await Program.DbContext.WriteToDb(pkt, NAME, replay);

        var result = await Program.DbContext.ReadFromDb($"SELECT Id FROM {NAME} WHERE PKT='{pkt}'");
        return replay with { Id = (uint)result.Last()["Id"] };
    }
}
