namespace api.sc2_directstrike.DTOs;

public record ReplayPlayer
{
    public int Id { get; init; }
    public int PlayerId { get; init; }

    public string Commander { get; init; } = null!;
    public int Team { get; init; }
    public int Position { get; init; }
}
