using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("{pkt}/" + NAME)]
public class PlayerController : ControllerBase
{
    public const string NAME = "players";

    [HttpGet("{id}")]
    public async Task<Player?> GetById(string pkt, int id)
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
    public async Task<IEnumerable<Player?>> Get(string pkt,
                                                [FromQuery(Name = "name")] string? name,
                                                [FromQuery(Name = "inGameId")] int? inGameId)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "Name", name);
        query += DbContext.AddCondition(query, "InGameId", inGameId);

        var result = await Program.DbContext.ReadFromDb(query);

        return result.Select(entry => (Player)entry!);
    }


    public static async Task<Player> GenerateIncrementedPlayer(string pkt, PostPlayer postPlayer)
    {
        var result = await Program.DbContext.ReadFromDb($"SELECT * FROM {NAME} WHERE PKT='{pkt}' AND InGameId='{postPlayer.InGameId}'")!;
        if (result.Any())
        {
            return result.Single()!;
        }

        Player player = postPlayer;
        await Program.DbContext.WriteToDb(pkt, NAME, player);

        result = await Program.DbContext.ReadFromDb($"SELECT Id FROM {NAME} WHERE PKT='{pkt}'");
        return player with { Id = (int)result.Last()["Id"] };
    }
}
