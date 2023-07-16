using Microsoft.AspNetCore.Mvc;

namespace sc2_directstrike.api.Controllers;
using DTOs;
using Contexts;

[ApiController]
[Route("{pkt}/" + PlayerContext.Table)]
public class PlayerController : ControllerBase
{
    private readonly IServiceProvider serviceProvider;

    public PlayerController(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [HttpGet("{id}")]
    public async Task<Player?> GetById(string pkt, ulong id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var playerContext = scope.ServiceProvider.GetRequiredService<PlayerContext>();

        var result = await playerContext.Get(pkt, conditions: new string[] { $"Id = '{id}'" }, "*");
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
                                              [FromQuery(Name = "inGameId")] ulong? inGameId = null)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var playerContext = scope.ServiceProvider.GetRequiredService<PlayerContext>();

        var conditions = new List<string>();
        if (name != null)
        {
            conditions.Add($"Name = '{name}'");
        }
        if (inGameId != null)
        {
            conditions.Add($"InGameId = '{inGameId}'");
        }

        var result = await playerContext.Get(pkt, conditions, "*");

        return result.Select(entry => (Player)entry!);
    }

    [HttpDelete]
    public async Task Delete(string pkt,
                            [FromQuery(Name = "name")] string? name = null,
                            [FromQuery(Name = "inGameId")] ulong? inGameId = null)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"DELETE " +
            $"FROM {PlayerContext.Table} " +
            $"WHERE PKT = {PKTController.GetQuery(pkt)} ";

        if (name != null)
        {
            query += $"AND Name = '{name}' ";
        }
        if (inGameId != null)
        {
            query += $"AND InGameId = '{inGameId}' ";
        }

        await dbContext.WriteToDb(query);
    }

    [HttpDelete("{id}")]
    public async Task Delete(string pkt, ulong id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        await dbContext.WriteToDb($"DELETE FROM {PlayerContext.Table} WHERE PKT = {PKTController.GetQuery(pkt)} AND Id = '{id}' ");
    }


    public static async Task<Player> GenerateIncrementedPlayer(string pkt, PostPlayer postPlayer, DbContext dbContext)
    {
        try
        {
            var result = await dbContext.ReadFromDb($"SELECT * FROM {PlayerContext.Table} WHERE PKT ={PKTController.GetQuery(pkt)} AND InGameId='{postPlayer.InGameId}'")!;
            if (result.Any())
            {
                return result.Single()!;
            }

            Player player = postPlayer;
            ulong id = await dbContext.WriteToDb(pkt, PlayerContext.Table, player);

            return player with { Id = id };
        }
        catch (Exception exp)
        {
            throw exp;
        }
    }
}
