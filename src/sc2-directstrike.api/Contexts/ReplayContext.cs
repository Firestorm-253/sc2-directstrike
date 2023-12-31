﻿namespace sc2_directstrike.api.Contexts;
using DTOs;
using Controllers;

public class ReplayContext
{
    public const string Table = "replays";

    private readonly IServiceProvider serviceProvider;

    public ReplayContext(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<Replay>> Get(string pkt, IEnumerable<string> conditions, params string[] selects)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"SELECT {string.Join(", ", selects)} " +
            $"FROM {Table} " +
            $"WHERE PKT = {PKTController.GetQuery(pkt)} ";

        if (conditions.Any())
        {
            query += $"AND {string.Join(" AND ", conditions)} ";
        }

        var result = await dbContext.ReadFromDb(query);

        return result.Select(entry => (Replay)entry!);
    }

    public async Task<Replay[]> GenerateIncrementedReplays(string pkt,
                                                           IEnumerable<Replay> replays,
                                                           DbContext dbContext,
                                                           MySqlConnector.MySqlTransaction? transaction = null)
    {
        ulong[] replay_ids = await dbContext.InsertIncremental(pkt, Table, replays, transaction);
        var replays_array = replays.ToArray();

        for (int i = 0; i < replays.Count(); i++)
        {
            replays_array[i] = replays_array[i] with { Id = replay_ids[i] };
        }
        return replays_array;
    }
}
