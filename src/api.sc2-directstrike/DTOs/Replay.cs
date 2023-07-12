using System.Text.Json;

namespace api.sc2_directstrike.DTOs;

public record Replay
{
    public uint Id { get; init; }
    public DateTime GameTime { get; init; }


    public static implicit operator Dictionary<string, object>(Replay replay)
        => new()
        {
            { "Id", replay.Id },
            { "GameTime", replay.GameTime.ToString("yyyy-dd-MM hh:mm:ss") },
        };

    public static implicit operator Replay? (Dictionary<string, object> dict)
    {
        if (dict == null)
        {
            return null;
        }

        return new Replay()
        {
            Id = (uint)dict["Id"],
            GameTime = (DateTime)dict["GameTime"],
        };
    }
}

public record PostReplay
{
    public DateTime GameTime { get; init; }
    public PostReplayPlayer[] ReplayPlayers { get; init; } = null!;


    public static implicit operator Replay(PostReplay postReplay)
        => new()
        {
            GameTime = postReplay.GameTime,
        };
}
