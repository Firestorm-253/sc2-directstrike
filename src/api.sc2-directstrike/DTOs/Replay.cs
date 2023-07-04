namespace api.sc2_directstrike.DTOs;

public record Replay
{
    public int Id { get; init; }
    public DateTime GameTime { get; init; }
    public ReplayPlayer[] ReplayPlayers { get; init; } = null!;
}
