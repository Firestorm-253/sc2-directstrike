namespace sc2_directstrike.api.Services.Data.RatingService.ProcessData;
using DTOs;

public record ReplayData
{
    public ReplayData(Replay replay, IEnumerable<ReplayPlayer> replayPlayers)
    {
        Replay = replay;

        var players_team1 = replayPlayers.Where(x => x.Team == 1);
        var players_team2 = replayPlayers.Where(x => x.Team == 2);

        Team1 = new(replay, players_team1, players_team1.Any(rp => rp.Result == 1));
        Team2 = new(replay, players_team2, players_team2.Any(rp => rp.Result == 1));
    }

    public Replay Replay { get; init; }
    
    public TeamData Team1 { get; init; }
    public TeamData Team2 { get; init; }

    public double Confidence { get; set; }

    public bool CorrectPrediction => Team1.IsWinner ? (Team1.ExpectedResult > 0.50) : (Team2.ExpectedResult > 0.50);
}