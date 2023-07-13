using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace sc2_directstrike.api.Contexts;
using DTOs;
using Controllers;

public class ReplayPlayerContext
{
    public const string Table = "replay_players";

    private readonly IServiceProvider serviceProvider;

    public ReplayPlayerContext(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<ReplayPlayer>> Get(string pkt, IEnumerable<string> conditions, params string[] selects)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"SELECT {string.Join(", ", selects)} " +
            $"FROM {Table} " +
            $"WHERE PKT = '{pkt}' ";

        if (conditions.Any())
        {
            query += $"AND {string.Join(" AND ", conditions)} ";
        }

        var result = await dbContext.ReadFromDb(query);

        return result.Select(entry => (ReplayPlayer)entry!);
    }
}
