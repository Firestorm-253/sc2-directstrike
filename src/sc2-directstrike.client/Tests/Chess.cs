namespace sc2_directstrike.client.Tests;
using DTOs;
using System.Diagnostics;

public static class Chess
{
    const string localEP1 = "https://localhost:7045/";
    const string localEP2 = "https://localhost:5001/";
    const string publicEP = "https://api.sc2-directstrike.com/";

    public static void Run()
    {
        var apiCommunicator = new ApiCommunicator(localEP2);
        string pkt = apiCommunicator.Get<string>("pkt")!;

        var allReplayChunks = new List<List<PostReplay>>();
        var allPlayers = new Dictionary<string, ulong>();

        string root = @"C:\Users\Zehnder\Downloads\KingBase2019-pgn";
        foreach (var file in Directory.GetFiles(root))
        {
            var sw = Stopwatch.StartNew();
            var currentReplayCunks = LoadFromPGNs(file, allPlayers);
            allReplayChunks.AddRange(currentReplayCunks);
            sw.Stop();
            Console.WriteLine($"{Path.GetFileName(file)} -> {sw.ElapsedMilliseconds}ms");
        }

        var sum = 0;
        foreach (var replayChunk in allReplayChunks)
        {
            sum += replayChunk.Count;

            var sw = Stopwatch.StartNew();
            var currentTries = 0;
            while (!apiCommunicator.Post($"{pkt}/replays", replayChunk.ToArray()).GetAwaiter().GetResult() && currentTries++ < 10)
            {
                Thread.Sleep(20000);
            }
            sw.Stop();
            Console.WriteLine($"{sum}/{(allReplayChunks.Count * replayChunk.Count)} -> {sw.ElapsedMilliseconds}ms");
        }
        Console.WriteLine("Finished");
    }

    public static List<List<PostReplay>> LoadFromPGNs(string file, Dictionary<string, ulong> allPlayers)
    {
        string fileText = File.ReadAllText(file);
        var allGames = fileText.Split("\r\n\r\n").Where(x => x.StartsWith('[')).ToArray();

        var replayChunk = new List<PostReplay>();
        var allReplayChunks = new List<List<PostReplay>>();

        foreach (var game in allGames)
        {
            var infos = game
                .Split("\r\n")
                .Select(x => x
                    .Trim('[', ']')
                    .Replace(" \"", ":")
                    .Replace("\"", ""))
                .ToDictionary(x => x.Split(":")[0]);

            foreach (var info in infos)
            {
                infos[info.Key] = info.Value.Replace($"{info.Key}:", string.Empty);
            }

            var result = infos["Result"];
            if (!(result == "1-0" || result == "0-1"))
            {
                continue;
            }

            if (!DateTime.TryParse(infos["Date"], out var date))
            {
                continue;
            }

            var whitePlayerName = infos["White"].Replace("'", " ");
            var blackPlayerName = infos["Black"].Replace("'", " ");

            if (whitePlayerName == blackPlayerName)
            {
                continue;
            }

            if (!allPlayers.ContainsKey(whitePlayerName))
            {
                allPlayers.Add(whitePlayerName, (ulong)allPlayers.Count);
            }
            if (!allPlayers.ContainsKey(blackPlayerName))
            {
                allPlayers.Add(blackPlayerName, (ulong)allPlayers.Count);
            }

            var whitePlayer = new PostReplayPlayer(
                new PostPlayer(allPlayers[whitePlayerName], whitePlayerName),
                1, 1, uint.Parse(result[0].ToString()), int.MaxValue, "white");
            var blackPlayer = new PostReplayPlayer(
                new PostPlayer(allPlayers[blackPlayerName], blackPlayerName),
                2, 1, uint.Parse(result[2].ToString()), int.MaxValue, "black");

            var replay = new PostReplay(date, "Chess", int.MaxValue, new[] { whitePlayer, blackPlayer });
            replayChunk.Add(replay);

            if (replayChunk.Count == 1_000)
            {
                allReplayChunks.Add(new List<PostReplay>(replayChunk));
                replayChunk.Clear();
            }
        }

        return allReplayChunks;
    }
}
