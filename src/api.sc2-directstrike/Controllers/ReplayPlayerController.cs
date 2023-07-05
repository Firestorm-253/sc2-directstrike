using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("replayPlayers")]
public class ReplayPlayerController : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ReplayPlayer?>> Get()
    {
        string query =
            $"SELECT * " +
            $"FROM replay_players ";

        var result = await DbContext.ReadFromDb(query);

        return result.Select(entry => Create(entry));
    }

    [HttpGet("{id}")]
    public async Task<ReplayPlayer?> GetById(int id)
    {
        string query =
            $"SELECT * " +
            $"FROM replay_players " +
            $"WHERE Id='{id}' ";

        var result = await DbContext.ReadFromDb(query);
        var entry = result.Single();

        return Create(entry);
    }

    [HttpGet("replayId={replayId}")]
    public async Task<IEnumerable<ReplayPlayer?>> GetByReplayId(int replayId)
    {
        string query =
            $"SELECT * " +
            $"FROM replay_players " +
            $"WHERE ReplayId='{replayId}' ";

        var result = await DbContext.ReadFromDb(query);

        return result.Select(entry => Create(entry));
    }

    [HttpGet("playerId={playerId}")]
    public async Task<IEnumerable<ReplayPlayer?>> GetByPlayerId(int playerId)
    {
        string query =
            $"SELECT * " +
            $"FROM replay_players " +
            $"WHERE PlayerId='{playerId}' ";

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
