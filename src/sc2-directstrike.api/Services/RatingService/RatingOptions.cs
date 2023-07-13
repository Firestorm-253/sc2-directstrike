namespace sc2_directstrike.api.Services.Data.RatingService;

public record RatingOptions
{
    public int DoubleAtDays { get; init; }
    public float EloK { get; init; }
    public float StartRating { get; init; }
    public float StandardMatchDeviation { get; init; }
    public float StandardPlayerDeviation { get; init; }
    public float BalanceDeviationOffset { get; init; }

    public static RatingOptions Default => new()
    {
        DoubleAtDays = 365,
        StartRating = 1000,
        EloK = 32,
        StandardMatchDeviation = 400,
        StandardPlayerDeviation = 400,
        BalanceDeviationOffset = 0,
    };
}
