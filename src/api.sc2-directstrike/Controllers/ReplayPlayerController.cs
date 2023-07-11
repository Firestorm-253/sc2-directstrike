using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;
using System.Numerics;

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
    public async Task<ReplayPlayer?> GetById(string pkt, int id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "Id", id);

        var result = await dbContext.ReadFromDb(query);
        var entry = result.Single();

        return entry;
    }

    [HttpGet]
    public async Task<IEnumerable<ReplayPlayer?>> Get(string pkt,
                                                      [FromQuery(Name = "replayId")] int? replayId,
                                                      [FromQuery(Name = "playerId")] int? playerId)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "ReplayId", replayId);
        query += DbContext.AddCondition(query, "PlayerId", playerId);

        var result = await dbContext.ReadFromDb(query);

        return result.Select(entry => (ReplayPlayer)entry!);
    }


    public static async Task<ReplayPlayer> GenerateIncrementedReplay(string pkt, PostReplayPlayer postReplayPlayer, Replay replay, Player player, DbContext dbContext)
    {
        ReplayPlayer replayPlayer = ((ReplayPlayer)postReplayPlayer) with
        {
            ReplayId = replay.Id,
            PlayerId = player.Id,
        };
        await dbContext.WriteToDb(pkt, NAME, replayPlayer);
        
        var result = await dbContext.ReadFromDb($"SELECT Id FROM {NAME} WHERE PKT='{pkt}'");
        return replayPlayer with { Id = (uint)result.Last()["Id"], };
    }
}
