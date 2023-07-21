using Microsoft.AspNetCore.Mvc;

namespace sc2_directstrike.api.Controllers;
using DTOs;
using Contexts;
using MySqlConnector;

[ApiController]
[Route("{pkt}/" + ReplayContext.Table)]
public class ReplayController : ControllerBase
{
    private readonly IServiceProvider serviceProvider;

    public ReplayController(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [HttpGet]
    public async Task<IEnumerable<Replay>> Get(string pkt,
                                              [FromQuery(Name = "gameMode")] string? gameMode = null)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var replayContext = scope.ServiceProvider.GetRequiredService<ReplayContext>();

        var conditions = new List<string>();
        if (gameMode != null)
        {
            conditions.Add($"GameMode = '{gameMode}'");
        }

        return await replayContext.Get(pkt, conditions, selects: "*");
    }

    [HttpGet("{id}")]
    public async Task<Replay?> GetById(string pkt, ulong id)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = serviceProvider.CreateScope();
        var replayContext = scope.ServiceProvider.GetRequiredService<ReplayContext>();

        var result = await replayContext.Get(pkt, conditions: new string[] { $"Id = '{id}'" }, "*");
        var entry = result.SingleOrDefault();

        if (entry == null)
        {
            return null;
        }
        return entry;
    }

    [HttpPost]
    public async Task Post(string pkt, [FromBody] IEnumerable<PostReplay> postReplays)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var replayContext = scope.ServiceProvider.GetRequiredService<ReplayContext>();
        var playerContext = scope.ServiceProvider.GetRequiredService<PlayerContext>();
        var replayPlayerContext = scope.ServiceProvider.GetRequiredService<ReplayPlayerContext>();

        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        using var transaction = await dbContext.Connection.BeginTransactionAsync();

        try
        {
            var replays = new Replay[postReplays.Count()];
            var replayPackages = new (PostReplay, Replay)[replays.Length];
            for (int i = 0; i < replays.Length; i++)
            {
                replays[i] = postReplays.ElementAt(i);
                replayPackages[i] = (postReplays.ElementAt(i), replays[i]);
            }

            var incrementedReplays =
                await replayContext.GenerateIncrementedReplays(
                    pkt,
                    replays,
                    dbContext,
                    transaction);

            var incrementedPlayers =
                await playerContext.GenerateIncrementedPlayers(
                    pkt,
                    postReplays.SelectMany(r => r.ReplayPlayers.Select(rp => (Player)rp.Player)),
                    dbContext,
                    transaction);

            await replayPlayerContext.GenerateIncrementedReplayPlayers(
                pkt,
                postReplays,
                incrementedReplays,
                incrementedPlayers,
                dbContext,
                transaction);
        }
        catch (Exception exp)
        {
            await transaction.RollbackAsync();
            throw exp;
        }

        await transaction.CommitAsync();
    }

    [HttpDelete]
    public async Task Delete(string pkt,
                            [FromQuery(Name = "gameMode")] string? gameMode = null)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"DELETE " +
            $"FROM {ReplayContext.Table} " +
            $"WHERE PKT = {PKTController.GetQuery(pkt)} ";

        if (gameMode != null)
        {
            query += $"AND GameMode = '{gameMode}' ";
        }

        await dbContext.ExecuteQuery(query);
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

        await dbContext.ExecuteQuery($"DELETE FROM {ReplayContext.Table} WHERE PKT = {PKTController.GetQuery(pkt)} AND Id = '{id}' ");
    }
}
