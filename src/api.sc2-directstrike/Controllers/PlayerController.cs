using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[Route(ROUTE)]
[ApiController]
public class PlayerController : ControllerBase
{
    const string ROUTE = "players";

    [HttpGet("{id}")]
    public async Task<Player?> GetById(int id)
    {
        string query =
            $"SELECT * " +
            $"FROM {ROUTE} " +
            $"WHERE Id='{id}' ";
        
        var result = await DbContext.ReadFromDb(query);
        var entry = result.Single();

        return Create(entry);
    }

    [HttpGet]
    public async Task<IEnumerable<Player?>> Get([FromQuery(Name = "name")] string? name,
                                                [FromQuery(Name = "toonId")] int? toonId)
    {
        string query =
            $"SELECT * " +
            $"FROM {ROUTE} ";

        query += query.AddCondition("Name", name);
        query += query.AddCondition("ToonId", toonId);

        var result = await DbContext.ReadFromDb(query);

        return result.Select(entry => Create(entry));
    }

    private static Player? Create(Dictionary<string, object>? entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new Player()
        {
            Id = (int)entry["Id"],
            ToonId = (int)entry["ToonId"],
            Name = (string)entry["Name"],
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
