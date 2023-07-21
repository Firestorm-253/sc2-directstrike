using System.Text.Json;

namespace sc2_directstrike.api.DTOs;

public record Replay : IAsDictionary
{
    public ulong Id { get; init; }
    public DateTime GameTime { get; init; }
    public string GameMode { get; init; } = null!;
    public uint Duration { get; init; }


    public Dictionary<string, object> AsDictionary()
        => new Dictionary<string, object>()
        {
            { "Id", this.Id },
            { "GameTime", this.GameTime.ToString("yyyy-MM-dd hh:mm:ss") },
            { "GameMode", this.GameMode },
            { "Duration", this.Duration },
        };

    public static implicit operator Dictionary<string, object>(Replay replay)
        => replay.AsDictionary();

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
