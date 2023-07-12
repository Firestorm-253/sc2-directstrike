using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("{pkt}/" + NAME)]
public class PlayerController : ControllerBase
{
    public const string NAME = "players";

    private readonly IServiceProvider serviceProvider;

    public PlayerController(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [HttpGet("{id}")]
    public async Task<Player?> GetById(string pkt, int id)
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
        var entry = result.SingleOrDefault();

        if (entry == null)
        {
            return null;
        }
        return entry;
    }

    [HttpGet]
    public async Task<IEnumerable<Player>> Get(string pkt,
                                                [FromQuery(Name = "name")] string? name = null,
                                                [FromQuery(Name = "inGameId")] int? inGameId = null)
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
        query += DbContext.AddCondition(query, "Name", name);
        query += DbContext.AddCondition(query, "InGameId", inGameId);

        var result = await dbContext.ReadFromDb(query);

        return result.Select(entry => (Player)entry!);
    }


    public static async Task<Player> GenerateIncrementedPlayer(string pkt, PostPlayer postPlayer, DbContext dbContext)
    {
        var result = await dbContext.ReadFromDb($"SELECT * FROM {NAME} WHERE PKT='{pkt}' AND InGameId='{postPlayer.InGameId}'")!;
        if (result.Any())
        {
            return result.Single()!;
        }

        Player player = postPlayer;
        await dbContext.WriteToDb(pkt, NAME, player);

        result = await dbContext.ReadFromDb($"SELECT Id FROM {NAME} WHERE PKT='{pkt}'");
        return player with { Id = (uint)result.Last()["Id"] };
    }
}
