using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("{pkt}/" + NAME)]
public class ReplayPlayerController : ControllerBase
{
    const string NAME = "replay_players";

    [HttpGet("{id}")]
    public async Task<ReplayPlayer?> GetById(int id)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += query.AddCondition("Id", id);
        var result = await DbContext.ReadFromDb(query);
        var entry = result.Single();

        return Create(entry);
    }

    [HttpGet]
    public async Task<IEnumerable<ReplayPlayer?>> Get([FromQuery(Name = "replayId")] int? replayId,
                                                [FromQuery(Name = "playerId")] int? playerId)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += query.AddCondition("ReplayId", replayId);
        query += query.AddCondition("PlayerId", playerId);

        var result = await DbContext.ReadFromDb(query);

        return result.Select(entry => Create(entry));
    }

    private static ReplayPlayer? Create(Dictionary<string, object>? entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new ReplayPlayer()
        {
            Id = (int)entry["Id"],
            PlayerId = (int)entry["PlayerId"],
            ReplayId = (int)entry["ReplayId"],

            Commander = (string)entry["Commander"],
            Team = (int)entry["Team"],
            Position = (int)entry["Position"],
        };
    }

    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
