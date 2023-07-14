using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

var apiCommunicator = new ApiCommunicator();
string pkt = apiCommunicator.Get<string>("pkt")!;

var replays = apiCommunicator.Get<IEnumerable<Replay>>($"{pkt}/replays");
var replay_players = apiCommunicator.Get<IEnumerable<ReplayPlayer>>($"{pkt}/replay_players");
var players = apiCommunicator.Get<IEnumerable<Player>>($"{pkt}/players");

var playerA = new PostPlayer(1, "A");
var playerB = new PostPlayer(2, "B");
var playerC = new PostPlayer(3, "C");
var playerD = new PostPlayer(4, "D");
var playerE = new PostPlayer(5, "E");
var playerF = new PostPlayer(6, "F");
apiCommunicator.Post($"{pkt}/replays", new PostReplay[]
{
    new(DateTime.UtcNow.AddMinutes(0), "DirectStrike_Commanders_3v3", 1200, new PostReplayPlayer[]
    {
        new(playerA, 1, 1, 0, 1200, ""),
        new(playerB, 1, 2, 0, 1200, ""),
        new(playerC, 1, 3, 0, 1200, ""),
        new(playerD, 2, 1, 1, 1200, ""),
        new(playerE, 2, 2, 1, 1200, ""),
        new(playerF, 2, 3, 1, 1200, ""),
    }),
    new(DateTime.UtcNow.AddMinutes(1), "DirectStrike_Commanders_3v3", 1200, new PostReplayPlayer[]
    {
        new(playerA, 1, 1, 0, 1200, ""),
        new(playerB, 1, 2, 0, 1200, ""),
        new(playerC, 1, 3, 0, 1200, ""),
        new(playerD, 2, 1, 1, 1200, ""),
        new(playerE, 2, 2, 1, 1200, ""),
        new(playerF, 2, 3, 1, 1200, ""),
    }),
    new(DateTime.UtcNow.AddMinutes(2), "DirectStrike_Commanders_3v3", 1200, new PostReplayPlayer[]
    {
        new(playerA, 1, 1, 0, 1200, ""),
        new(playerB, 1, 2, 0, 1200, ""),
        new(playerC, 1, 3, 0, 1200, ""),
        new(playerD, 2, 1, 1, 1200, ""),
        new(playerE, 2, 2, 1, 1200, ""),
        new(playerF, 2, 3, 1, 1200, ""),
    }),
    new(DateTime.UtcNow.AddMinutes(3), "DirectStrike_Commanders_3v3", 1200, new PostReplayPlayer[]
    {
        new(playerA, 1, 1, 0, 1200, ""),
        new(playerB, 1, 2, 0, 1200, ""),
        new(playerC, 1, 3, 0, 1200, ""),
        new(playerD, 2, 1, 1, 1200, ""),
        new(playerE, 2, 2, 1, 1200, ""),
        new(playerF, 2, 3, 1, 1200, ""),
    }),
}).Wait();

replays = apiCommunicator.Get<IEnumerable<Replay>>($"{pkt}/replays");
replay_players = apiCommunicator.Get<IEnumerable<ReplayPlayer>>($"{pkt}/replay_players");
players = apiCommunicator.Get<IEnumerable<Player>>($"{pkt}/players");

if (apiCommunicator.Execute($"{pkt}/rating"))
{
    replays = apiCommunicator.Get<IEnumerable<Replay>>($"{pkt}/replays");
    replay_players = apiCommunicator.Get<IEnumerable<ReplayPlayer>>($"{pkt}/replay_players");
    players = apiCommunicator.Get<IEnumerable<Player>>($"{pkt}/players");
}

Console.ReadKey();


public class ApiCommunicator
{
    private readonly HttpClient client;

    public ApiCommunicator()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.sc2-directstrike.com/")
        };
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public bool Execute(string requestUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        var response = client.SendAsync(request).GetAwaiter().GetResult();

        return response.IsSuccessStatusCode;
    }

    public T? Get<T>(string requestUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        var response = client.SendAsync(request).GetAwaiter().GetResult();

        if (response.IsSuccessStatusCode)
        {
            return response.Content.ReadAsAsync<T>().GetAwaiter().GetResult();
        }
        return default;
    }

    public async Task Post<T>(string requestUrl, T[] objs)
    {
        var response = await client.PostAsJsonAsync(requestUrl, objs);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("ERROR: Post failed!");
        }
    }

    public void Delete(string requestUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);

        var response = client.SendAsync(request).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("ERROR: Delete failed!");
        }
    }
}

public record Replay(
    ulong Id,
    DateTime GameTime,
    string GameMode,
    uint Duration
);
public record PostReplay(
    DateTime GameTime,
    string GameMode,
    uint Duration,
    PostReplayPlayer[] ReplayPlayers
);

public record ReplayPlayer(
    ulong Id,
    ulong PlayerId,
    ulong ReplayId,
    uint Team,
    uint Position,
    uint Result,
    uint Duration,
    string Commander,
    float RatingBefore,
    float RatingAfter,
    float DeviationBefore,
    float DeviationAfter
);
public record PostReplayPlayer(
    PostPlayer Player,
    uint Team,
    uint Position,
    uint Result,
    uint Duration,
    string Commander
);

public record Player(
    ulong Id,
    ulong InGameId,
    string Name
);

public record PostPlayer(
    ulong InGameId,
    string Name
);
