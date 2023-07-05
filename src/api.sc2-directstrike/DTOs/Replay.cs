namespace api.sc2_directstrike.DTOs;

public record Replay
{
    public int Id { get; init; }
    public DateTime GameTime { get; init; }
    public int[] ReplayPlayersIds { get; init; } = null!;
}
