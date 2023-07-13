namespace sc2_directstrike.api.Services.Data.RatingService.ProcessData;
using DTOs;
using Maths;

public record PlayerData
{
    public PlayerData(Replay replay, ReplayPlayer replayPlayer)
    {
        ReplayPlayer = replayPlayer;
        Deltas = new();

        IsLeaver = replayPlayer.Duration < replay.Duration - 90;
    }

    public ReplayPlayer ReplayPlayer { get; init; }

    public bool IsLeaver { get; init; }

    public TimeSpan TimeSinceLastGame { get; set; }

    public float Rating { get; set; }
    public float Deviation { get; set; }
    public float Confidence => (float)Gaussian.GetPrecision(Deviation);

    public Gaussian Distribution => Gaussian.ByMeanDeviation(Rating, Deviation);

    public PlayerDeltas Deltas { get; init; }
}

public record PlayerDeltas
{
    public float Rating { get; set; }
    public float Deviation { get; set; }
}