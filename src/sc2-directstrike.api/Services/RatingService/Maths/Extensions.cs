namespace sc2_directstrike.api.Services.Data.RatingService.Maths;

public static class Extensions
{
    public static double Sqrt(this double value) => Math.Sqrt(value);
    public static double Pow(this double value, double exp = 2) => Math.Pow(value, exp);
}
