namespace api.sc2_directstrike.DTOs;

public record ReplayPlayer
{
    //public int Id { get; init; }
    public int PlayerId { get; init; }
    public int ReplayId { get; init; }

    public int Team { get; init; }
    public int Position { get; init; }
    public string Commander { get; init; } = null!;

    public static implicit operator ReplayPlayer?(Dictionary<string, object> entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new ReplayPlayer()
        {
            //Id = (int)entry["Id"],
            PlayerId = (int)entry["PlayerId"],
            ReplayId = (int)entry["ReplayId"],

            Team = (int)entry["Team"],
            Position = (int)entry["Position"],
            Commander = (string)entry["Commander"],
        };
    }
}
