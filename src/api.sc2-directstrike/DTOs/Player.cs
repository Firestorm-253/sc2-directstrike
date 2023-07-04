namespace api.sc2_directstrike.DTOs;

public record Player
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
}
