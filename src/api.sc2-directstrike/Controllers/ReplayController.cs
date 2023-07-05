using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("replays")]
public class ReplayController : ControllerBase
{
    public ReplayController() { }

    [HttpGet]
    public async Task<IEnumerable<Replay?>> Get()
    {
        string query =
            $"SELECT * " +
            $"FROM replays ";

        var result = await DbContext.ReadFromDb(query);

        return result.Select(entry => Create(entry));
    }

    [HttpGet("id={id}")]
    public async Task<Replay?> GetById(int id)
    {
        string query =
            $"SELECT * " +
            $"FROM replays " +
            $"WHERE Id='{id}' ";

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

    [HttpPost()]
    public async Task Post(Replay replay)
    {
        await DbContext.WriteToDb($"INSERT INTO replays (Id, GameTime) " +
                                  $"VALUES ({replay.Id}, '{replay.GameTime:yyyy-dd-MM hh:mm:ss}') ");
    }
}
