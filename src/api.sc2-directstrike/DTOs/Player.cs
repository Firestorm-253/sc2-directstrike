namespace api.sc2_directstrike.DTOs;

public record Player
{
    public int Id { get; init; }
    public int ToonId { get; init; }
    public string Name { get; init; } = null!;

    public static implicit operator Player?(Dictionary<string, object> entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new Player()
        {
            Id = (int)entry["Id"],
            ToonId = (int)entry["ToonId"],
            Name = (string)entry["Name"],
        };
    }
}
