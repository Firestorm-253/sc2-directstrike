namespace api.sc2_directstrike.DTOs;

public record Player
{
    public int Id { get; init; }
    public int InGameId { get; init; }
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
            Id = (int)entry["Id"],
            InGameId = (int)entry["InGameId"],
            Name = (string)entry["Name"],
        };
    }
}

public record PostPlayer
{
    public int InGameId { get; init; }
    public string Name { get; init; } = null!;


    public static implicit operator Player(PostPlayer postPlayer)
        => new()
        {
            InGameId = postPlayer.InGameId,
            Name = postPlayer.Name,
        };
}
