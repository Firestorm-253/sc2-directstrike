namespace sc2_directstrike.api.DTOs;

public record ReplayPlayerRating
{
    public ulong Id { get; init; }
    public ulong ReplayPlayerId { get; init; }

    public float RatingBefore { get; set; }
    public float RatingAfter { get; set; }
    public float DeviationBefore { get; set; }
    public float DeviationAfter { get; set; }

    public static implicit operator Dictionary<string, object>(ReplayPlayerRating rating)
        => new()
        {
            { "Id", rating.Id },
            { "ReplayPlayerId", rating.ReplayPlayerId },

            { "RatingBefore", rating.RatingBefore },
            { "RatingAfter", rating.RatingAfter },
            { "DeviationBefore", rating.DeviationBefore },
            { "DeviationAfter", rating.DeviationAfter },
        };

    public static implicit operator ReplayPlayerRating?(Dictionary<string, object> entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new ReplayPlayerRating()
        {
            Id = (ulong)entry["Id"],
            ReplayPlayerId = (ulong)entry["ReplayPlayerId"],

            RatingBefore = (float)entry["RatingBefore"],
            RatingAfter = (float)entry["RatingAfter"],
            DeviationBefore = (float)entry["DeviationBefore"],
            DeviationAfter = (float)entry["DeviationAfter"],
        };
    }
}
