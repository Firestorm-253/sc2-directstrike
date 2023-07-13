namespace sc2_directstrike.api.Services;
using Data.RatingService;

public partial class RatingService
{
    private static double GetDecayFactor(TimeSpan timeSpan, RatingOptions ratingOptions)
    {
        return 1 + (timeSpan.TotalDays / ratingOptions.DoubleAtDays);
    }
}
