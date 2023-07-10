namespace api.sc2_directstrike.DTOs;

public record Replay
{
    //public int Id { get; init; }
    public DateTime GameTime { get; init; }
    //public int[] ReplayPlayersIds { get; init; } = null!;

    public static implicit operator Dictionary<string, object>(Replay replay)
        => new()
        {
            //{ "Id", replay.Id },
            { "GameTime", replay.GameTime.ToString("yyyy-dd-MM hh:mm:ss") },
            //{ "ReplayPlayersIds", this.ReplayPlayersIds },
        };

    public static implicit operator Replay? (Dictionary<string, object> dict)
    {
        if (dict == null)
        {
            return null;
        }

        return new Replay()
        {
            //Id = (int)dict["Id"],
            GameTime = (DateTime)dict["GameTime"]
        };
    }
}
