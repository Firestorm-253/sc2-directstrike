using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using sc2_directstrike.client;

sc2_directstrike.client.Tests.Chess.Run();

//var apiCommunicator = new ApiCommunicator("https://api.sc2-directstrike.com/");
//string pkt = apiCommunicator.Get<string>("pkt")!;

//var replays = apiCommunicator.Get<IEnumerable<Replay>>($"{pkt}/replays");
//var replay_players = apiCommunicator.Get<IEnumerable<ReplayPlayer>>($"{pkt}/replay_players");
//var players = apiCommunicator.Get<IEnumerable<Player>>($"{pkt}/players");

//var playerA = new PostPlayer(1, "A");
//var playerB = new PostPlayer(2, "B");
//var playerC = new PostPlayer(3, "C");
//var playerD = new PostPlayer(4, "D");
//var playerE = new PostPlayer(5, "E");
//var playerF = new PostPlayer(6, "F");
//apiCommunicator.Post($"{pkt}/replays", new PostReplay[]
//{
//    new(DateTime.UtcNow.AddMinutes(0), "DirectStrike_Commanders_3v3", 1200, new PostReplayPlayer[]
//    {
//        new(playerA, 1, 1, 0, 1200, ""),
//        new(playerB, 1, 2, 0, 1200, ""),
//        new(playerC, 1, 3, 0, 1200, ""),
//        new(playerD, 2, 1, 1, 1200, ""),
//        new(playerE, 2, 2, 1, 1200, ""),
//        new(playerF, 2, 3, 1, 1200, ""),
//    }),
//    new(DateTime.UtcNow.AddMinutes(1), "DirectStrike_Commanders_3v3", 1200, new PostReplayPlayer[]
//    {
//        new(playerA, 1, 1, 0, 1200, ""),
//        new(playerB, 1, 2, 0, 1200, ""),
//        new(playerC, 1, 3, 0, 1200, ""),
//        new(playerD, 2, 1, 1, 1200, ""),
//        new(playerE, 2, 2, 1, 1200, ""),
//        new(playerF, 2, 3, 1, 1200, ""),
//    }),
//    new(DateTime.UtcNow.AddMinutes(2), "DirectStrike_Commanders_3v3", 1200, new PostReplayPlayer[]
//    {
//        new(playerA, 1, 1, 0, 1200, ""),
//        new(playerB, 1, 2, 0, 1200, ""),
//        new(playerC, 1, 3, 0, 1200, ""),
//        new(playerD, 2, 1, 1, 1200, ""),
//        new(playerE, 2, 2, 1, 1200, ""),
//        new(playerF, 2, 3, 1, 1200, ""),
//    }),
//    new(DateTime.UtcNow.AddMinutes(3), "DirectStrike_Commanders_3v3", 1200, new PostReplayPlayer[]
//    {
//        new(playerA, 1, 1, 0, 1200, ""),
//        new(playerB, 1, 2, 0, 1200, ""),
//        new(playerC, 1, 3, 0, 1200, ""),
//        new(playerD, 2, 1, 1, 1200, ""),
//        new(playerE, 2, 2, 1, 1200, ""),
//        new(playerF, 2, 3, 1, 1200, ""),
//    }),
//}).Wait();

//replays = apiCommunicator.Get<IEnumerable<Replay>>($"{pkt}/replays");
//replay_players = apiCommunicator.Get<IEnumerable<ReplayPlayer>>($"{pkt}/replay_players");
//players = apiCommunicator.Get<IEnumerable<Player>>($"{pkt}/players");

//if (apiCommunicator.Execute($"{pkt}/rating", HttpMethod.Post))
//{
//    replays = apiCommunicator.Get<IEnumerable<Replay>>($"{pkt}/replays");
//    replay_players = apiCommunicator.Get<IEnumerable<ReplayPlayer>>($"{pkt}/replay_players");
//    players = apiCommunicator.Get<IEnumerable<Player>>($"{pkt}/players");
//}

//Console.ReadKey();


