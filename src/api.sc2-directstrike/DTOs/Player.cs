namespace api.sc2_directstrike.DTOs;

public record Player
{
    public uint Id { get; init; }
    public uint InGameId { get; init; }
    public string Name { get; init; } = null!;


    public static implicit operator Dictionary<string, object>(Player player)
        => new()
        {
            { "Id", player.Id },
            { "InGameId", player.InGameId },
            { "Name", player.Name },
        };

    public static implicit operator Player?(Dictionary<string, object> entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new Player()
        {
            Id = (uint)entry["Id"],
            InGameId = (uint)entry["InGameId"],
            Name = (string)entry["Name"],
        };
    }
}

public record PostPlayer
{
    public uint InGameId { get; init; }
    public string Name { get; init; } = null!;


    public static implicit operator Player(PostPlayer postPlayer)
        => new()
        {
            InGameId = postPlayer.InGameId,
            Name = postPlayer.Name,
        };
}
