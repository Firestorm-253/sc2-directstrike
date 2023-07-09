using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("{pkt}/" + NAME)]
public class ReplayController : ControllerBase
{
    const string NAME = "replays";

    [HttpGet]
    public async Task<IEnumerable<Replay?>> Get(string pkt)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += query.AddCondition("PKT", pkt);

        var result = await DbContext.ReadFromDb(query);

        return result.Select(entry => Create(entry));
    }

    [HttpGet("{id}")]
    public async Task<Replay?> GetById(string pkt, int id)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += query.AddCondition("PKT", pkt);
        query += query.AddCondition("Id", id);

        var result = await DbContext.ReadFromDb(query);
        var entry = result.Single();
        
        return Create(entry);
    }

    private static Replay? Create(Dictionary<string, object>? entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new Replay()
        {
            Id = (int)entry["Id"],
            GameTime = (DateTime)entry["GameTime"]
        };
    }

    [HttpPost]
    public async Task Post(string pkt, [FromBody] Replay replay)
    {
        await DbContext.WriteToDb(pkt, NAME, replay);
    }
}
