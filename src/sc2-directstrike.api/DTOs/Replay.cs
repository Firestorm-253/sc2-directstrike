using System.Text.Json;

namespace sc2_directstrike.api.DTOs;

public record Replay
{
    public ulong Id { get; init; }
    public DateTime GameTime { get; init; }
    public string GameMode { get; init; } = null!;
    public uint Duration { get; init; }


    public static implicit operator Dictionary<string, object>(Replay replay)
        => new()
        {
            { "Id", replay.Id },
            { "GameTime", replay.GameTime.ToString("yyyy-dd-MM hh:mm:ss") },
            { "GameMode", replay.GameMode },
            { "Duration", replay.Duration },
        };

    public static implicit operator Replay? (Dictionary<string, object> dict)
    {
        if (dict == null)
        {
            return null;
        }

        return new Replay()
        {
            Id = (ulong)dict["Id"],
            GameTime = (DateTime)dict["GameTime"],
            GameMode = (string)dict["GameMode"],
            Duration = (uint)dict["Duration"],
        };
    }
}

public record PostReplay
{
    public DateTime GameTime { get; init; }
    public string GameMode { get; init; } = null!;
    public uint Duration { get; init; }
    public PostReplayPlayer[] ReplayPlayers { get; init; } = null!;


    public static implicit operator Replay(PostReplay postReplay)
        => new()
        {
            GameTime = postReplay.GameTime,
            GameMode = postReplay.GameMode,
            Duration = postReplay.Duration,
        };
}
