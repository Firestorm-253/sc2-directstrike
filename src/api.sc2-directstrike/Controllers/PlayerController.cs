using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("{pkt}/" + NAME)]
public class PlayerController : ControllerBase
{
    const string NAME = "players";

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
                                                [FromQuery(Name = "toonId")] int? toonId)
    {
        string query =
            $"SELECT * " +
            $"FROM {NAME} ";

        query += DbContext.AddCondition(query, "PKT", pkt);
        query += DbContext.AddCondition(query, "Name", name);
        query += DbContext.AddCondition(query, "ToonId", toonId);

        var result = await Program.DbContext.ReadFromDb(query);

        return result.Select(entry => (Player)entry);
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
