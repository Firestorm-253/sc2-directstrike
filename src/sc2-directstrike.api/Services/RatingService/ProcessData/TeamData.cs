namespace sc2_directstrike.api.Services.Data.RatingService.ProcessData;
using DTOs;
using Maths;

public record TeamData
{
    public TeamData(Replay replay, IEnumerable<ReplayPlayer> replayPlayers, bool isWinner)
    {
        IsWinner = isWinner;
        ActualResult = isWinner ? 1 : 0;

        Players = replayPlayers.Select(p => new PlayerData(replay, p)).ToArray();
    }

    public PlayerData[] Players { get; init; }

    public bool IsWinner { get; init; }
    public int ActualResult { get; init; }

    public double Rating => Distribution.Mean;
    public double Deviation => Distribution.Deviation;
    public double Confidence => Distribution.Precision;

    public Gaussian Distribution { get; set; }
    public Gaussian Prediction { get; set; }

    public double ExpectedResult { get; set; }
}
