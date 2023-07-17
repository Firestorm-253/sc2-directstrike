namespace sc2_directstrike.api.Services.Data.RatingService.Maths;

public static class GaussianElo
{
    public static Gaussian GetRatingAfter(Gaussian rating,
                                          int actualResult,
                                          double prediction,
                                          Gaussian matchDist,
                                          double impact,
                                          bool isLeaver,
                                          RatingOptions ratingOptions)
    {
        double zScore_1 = matchDist.Mean + (1 * matchDist.Deviation);

        double deltaV2 = 0;
        if (actualResult == 1)
        {
            deltaV2 += Math.Max(0, zScore_1);
        }
        else
        {
            deltaV2 += Math.Min(0, zScore_1);
        }

        double delta = (actualResult - prediction) * ratingOptions.EloK * impact;

        var info = Gaussian.ByMeanDeviation(rating.Mean + deltaV2, matchDist.Deviation);
        var ratingAfter = rating * info;

        if (isLeaver)
        {
            ratingAfter = Gaussian.ByMeanDeviation(ratingAfter.Mean, rating.Deviation);
        }

        return ratingAfter;
    }

    public static (double, Gaussian) PredictMatch(Gaussian a, Gaussian b, double deviationOffset = 0)
    {
        var subtraction = b - a;
        var match = Gaussian.ByMeanDeviation(subtraction.Mean, subtraction.Deviation + deviationOffset);

        return (match.CDF(0), match);
    }
}
