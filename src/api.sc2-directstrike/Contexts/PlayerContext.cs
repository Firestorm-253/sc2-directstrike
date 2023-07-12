using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace api.sc2_directstrike.Contexts;
using DTOs;
using Controllers;

public class PlayerContext
{
    private readonly IServiceProvider serviceProvider;
    
    public PlayerContext(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<Player>> Get(string pkt, IEnumerable<string> conditions, params string[] selects)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string query =
            $"SELECT {string.Join(", ", selects)} " +
            $"FROM players " +
            $"WHERE PKT = '{pkt}' ";

        if (conditions.Any())
        {
            query += $"AND {string.Join(" AND ", conditions)} ";
        }

        var result = await dbContext.ReadFromDb(query);

        return result.Select(entry => (Player)entry!);
    }
}
