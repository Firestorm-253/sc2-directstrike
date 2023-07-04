namespace api.sc2_directstrike.DTOs;

public record ReplayPlayer
{
    public int Id { get; init; }
    public Player Player { get; init; } = null!;
    public string Commander { get; init; } = null!;
    public int Team { get; init; }
    public int Position { get; init; }
}
