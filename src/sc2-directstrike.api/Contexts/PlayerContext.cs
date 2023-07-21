namespace sc2_directstrike.api.Contexts;
using DTOs;
using Controllers;

public class PlayerContext
{
    public const string Table = "players";

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
            $"FROM {Table} " +
            $"WHERE PKT = {PKTController.GetQuery(pkt)} ";

        if (conditions.Any())
        {
            query += $"AND {string.Join(" AND ", conditions)} ";
        }

        var result = await dbContext.ReadFromDb(query);

        return result.Select(entry => (Player)entry!);
    }

    public async Task<Dictionary<ulong, Player>> GenerateIncrementedPlayers(string pkt,
                                                                            IEnumerable<Player> players,
                                                                            DbContext dbContext,
                                                                            MySqlConnector.MySqlTransaction? transaction = null)
    {
        ulong[] player_ids = await dbContext.InsertIncremental(pkt, Table, players, transaction);
        var players_inGameId = new Dictionary<ulong, Player>();

        for (int i = 0; i < players.Count(); i++)
        {
            var player = players.ElementAt(i);
            players_inGameId.Add(player.InGameId, player with { Id = player_ids[i] });
        }
        return players_inGameId;
    }
}
