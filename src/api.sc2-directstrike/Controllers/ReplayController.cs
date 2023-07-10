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
    public async Task Post(string pkt, [FromBody] Replay replay)
    {
        await Program.DbContext.WriteToDb(pkt, NAME, replay);
    }
}
