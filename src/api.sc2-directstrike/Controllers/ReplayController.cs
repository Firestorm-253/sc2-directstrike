using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

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
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);

        var result = await dbContext.ReadFromDb(query);

        return result.Select(entry => (Replay)entry!);
    }

    [HttpGet("{id}")]
    public async Task<Replay?> GetById(string pkt, int id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "Id", id);

        var result = await dbContext.ReadFromDb(query);
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
        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

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
        await dbContext.WriteToDb(pkt, NAME, replay);

        var result = await dbContext.ReadFromDb($"SELECT Id FROM {NAME} WHERE PKT='{pkt}'");
        return replay with { Id = (uint)result.Last()["Id"] };
    }
}
