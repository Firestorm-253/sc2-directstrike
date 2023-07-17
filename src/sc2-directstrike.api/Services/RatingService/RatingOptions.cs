namespace sc2_directstrike.api.Services.Data.RatingService;

public record RatingOptions
{
    public int DoubleAtDays { get; init; }
    public float StartRating { get; init; }
    public float EloK { get; init; } = float.NaN;
    public float StandardMatchDeviation { get; init; } = float.NaN;
    public float StandardPlayerDeviation { get; init; } = float.NaN;
    public float BalanceDeviationOffset { get; init; } = float.NaN;

    public RatingOptions Get(int playerAmount)
    {
        float standardPlayerDeviation = 1200;
        float standardMatchDeviation = MathF.Sqrt(2/*totalPlayerAmountPerGame*/ * MathF.Pow(standardPlayerDeviation, 2));

        return this with
        {
            StandardPlayerDeviation = standardPlayerDeviation,
            StandardMatchDeviation = standardMatchDeviation,
            BalanceDeviationOffset = standardMatchDeviation * 0f/*chess*/ /*0.0033f DS*/,
            EloK = standardPlayerDeviation * 0.75f,
        };
    }

    public static RatingOptions Default
        => new()
        {
            DoubleAtDays = 3650/*chess*/ /*440 DS*/,
            StartRating = 1000,
        };
}
