namespace api.sc2_directstrike.DTOs;

public record Replay
{
    public int Id { get; init; }
    public DateTime GameTime { get; init; }
    //public int[] ReplayPlayersIds { get; init; } = null!;

    public static implicit operator Dictionary<string, object>(Replay replay)
        => new()
        {
            { "Id", replay.Id },
            { "GameTime", replay.GameTime.ToString("yyyy-dd-MM hh:mm:ss") },
            //{ "ReplayPlayersIds", this.ReplayPlayersIds },
        };
}
