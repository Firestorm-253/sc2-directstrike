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
            $"WHERE PKT = {PKTController.GetQuery(pkt)} ";

        if (conditions.Any())
        {
            query += $"AND {string.Join(" AND ", conditions)} ";
        }

        var result = await dbContext.ReadFromDb(query);

        return result.Select(entry => (ReplayPlayer)entry!);
    }

    public async Task GenerateIncrementedReplayPlayers(string pkt,
                                                       IEnumerable<PostReplay> postReplays,
                                                       Replay[] replays,
                                                       Dictionary<ulong, Player> incrementedPlayers,
                                                       DbContext dbContext,
                                                       MySqlConnector.MySqlTransaction? transaction = null)
    {
        var replayPlayers = new List<ReplayPlayer>();
        for (int i = 0; i < replays.Length; i++)
        {
            foreach (var postReplayPlayer in postReplays.ElementAt(i).ReplayPlayers)
            {
                var player = incrementedPlayers[postReplayPlayer.Player.InGameId];

                var replayPlayer = ((ReplayPlayer)postReplayPlayer) with
                {
                    ReplayId = replays[i].Id,
                    PlayerId = player.Id
                };
                replayPlayers.Add(replayPlayer);
            }
        }
        
        await dbContext.InsertIncremental(pkt, Table, replayPlayers, transaction);
    }
}
