using Newtonsoft.Json;
using System.Net.Http.Headers;


var apiCommunicator = new ApiCommunicator();
string pkt = "FO6rBWmzz0nwcMXEyTPwzTSu"; //apiCommunicator.Get<string>("pkt")!;

apiCommunicator.Delete($"{pkt}/replay_players");


var replays = apiCommunicator.Get<IEnumerable<Replay>>($"{pkt}/replays");
var replay_players = apiCommunicator.Get<IEnumerable<ReplayPlayer>>($"{pkt}/replay_players");
var players = apiCommunicator.Get<IEnumerable<Player>>($"{pkt}/players");

var playerA = new PostPlayer(0, "A");
var playerB = new PostPlayer(1, "B");
var playerC = new PostPlayer(2, "C");

apiCommunicator.Post($"{pkt}/replays", new PostReplay[]
{
    new(DateTime.UtcNow.AddMinutes(0), new PostReplayPlayer[]
    {
        new(playerA, 0, 0, ""),
        new(playerB, 1, 1, ""),
    }),
    new(DateTime.UtcNow.AddMinutes(1), new PostReplayPlayer[]
    {
        new(playerA, 0, 0, ""),
        new(playerC, 1, 1, ""),
    }),
    new(DateTime.UtcNow.AddMinutes(2), new PostReplayPlayer[]
    {
        new(playerB, 0, 0, ""),
        new(playerC, 1, 1, ""),
    }),
});

replays = apiCommunicator.Get<IEnumerable<Replay>>($"{pkt}/replays");
replay_players = apiCommunicator.Get<IEnumerable<ReplayPlayer>>($"{pkt}/replay_players");
players = apiCommunicator.Get<IEnumerable<Player>>($"{pkt}/players");


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

    public void Post<T>(string requestUrl, T[] objs)
    {
        var json = JsonConvert.SerializeObject(objs);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = httpContent };

        var response = client.SendAsync(request).GetAwaiter().GetResult();
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
    DateTime GameTime
);
public record PostReplay(
    DateTime GameTime,
    PostReplayPlayer[] ReplayPlayers
);

public record ReplayPlayer(
    ulong Id,
    ulong PlayerId,
    ulong ReplayId,
    uint Team,
    uint Position,
    string Commander
);
public record PostReplayPlayer(
    PostPlayer Player,
    uint Team,
    uint Position,
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
