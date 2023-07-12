﻿using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using DTOs;
using Contexts;

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
                                                [FromQuery(Name = "inGameId")] int? inGameId = null)
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


    public static async Task<Player> GenerateIncrementedPlayer(string pkt, PostPlayer postPlayer, DbContext dbContext)
    {
        var result = await dbContext.ReadFromDb($"SELECT * FROM {NAME} WHERE PKT='{pkt}' AND InGameId='{postPlayer.InGameId}'")!;
        if (result.Any())
        {
            return result.Single()!;
        }

        Player player = postPlayer;
        uint id = await dbContext.WriteToDb(pkt, NAME, player);

        return player with { Id = id };
    }
}
