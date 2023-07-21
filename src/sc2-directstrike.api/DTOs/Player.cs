namespace sc2_directstrike.api.DTOs;

public record Player : IAsDictionary
{
    public ulong Id { get; init; }
    public ulong InGameId { get; init; }
    public string Name { get; init; } = null!;


    public Dictionary<string, object> AsDictionary()
        => new Dictionary<string, object>()
        {
            { "Id", this.Id },
            { "InGameId", this.InGameId },
            { "Name", this.Name },
        };

    public static implicit operator Dictionary<string, object>(Player player)
        => player.AsDictionary();

    public static implicit operator Player?(Dictionary<string, object> entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new Player()
        {
            Id = (ulong)entry["Id"],
            InGameId = (ulong)entry["InGameId"],
            Name = (string)entry["Name"],
        };
    }
}

public record PostPlayer
{
    public ulong InGameId { get; init; }
    public string Name { get; init; } = null!;


    public static implicit operator Player(PostPlayer postPlayer)
        => new()
        {
            InGameId = postPlayer.InGameId,
            Name = postPlayer.Name,
        };
}
