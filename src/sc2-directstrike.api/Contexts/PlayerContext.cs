namespace sc2_directstrike.api.Contexts;
using DTOs;
using Controllers;
using System.Xml.Linq;

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
        var existingPlayers_inGameId = new Dictionary<ulong, Player>();
        var existingPlayerDicts = await dbContext.ReadFromDb($"SELECT * FROM {Table} WHERE PKT = {PKTController.GetQuery(pkt)}", transaction)!;
        foreach (var existingPlayerDict in existingPlayerDicts)
        {
            Player existingPlayer = existingPlayerDict!;
            existingPlayers_inGameId.Add(existingPlayer.InGameId, existingPlayer);
        }

        var players_list = players.ToList();
        
        foreach (var player in players)
        {
            if (existingPlayers_inGameId.ContainsKey(player.InGameId))
            {
                players_list.Remove(player);
            }
        }

        ulong[] player_ids = await dbContext.InsertIncremental(pkt, Table, players_list, transaction);
        var players_inGameId = new Dictionary<ulong, Player>();

        for (int i = 0; i < players_list.Count; i++)
        {
            players_inGameId.Add(players_list[i].InGameId, players_list[i] with { Id = player_ids[i] });
        }

        for (int i = 0; i < players.Count(); i++)
        {
            var player = players.ElementAt(i);
            
            if (existingPlayers_inGameId.TryGetValue(player.InGameId, out var existingPlayer))
            {
                players_inGameId.TryAdd(player.InGameId, existingPlayer);
            }
        }

        return players_inGameId;
    }
}
