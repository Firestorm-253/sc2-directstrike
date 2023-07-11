using System.Text.Json;

namespace api.sc2_directstrike.DTOs;

public record Replay
{
    public uint Id { get; init; }
    public DateTime GameTime { get; init; }
    public int[] ReplayPlayersIds { get; init; } = null!;


    public static implicit operator Dictionary<string, object>(Replay replay)
        => new()
        {
            { "Id", replay.Id },
            { "GameTime", replay.GameTime.ToString("yyyy-dd-MM hh:mm:ss") },
            { "ReplayPlayersIds", JsonSerializer.Serialize(replay.ReplayPlayersIds, new JsonSerializerOptions() { WriteIndented = false}) },
        };

    public static implicit operator Replay? (Dictionary<string, object> dict)
    {
        if (dict == null)
        {
            return null;
        }

        return new Replay()
        {
            GameTime = (DateTime)dict["GameTime"],
            ReplayPlayersIds = JsonSerializer.Deserialize<int[]>((string)dict["ReplayPlayersIds"])!,
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
            ReplayPlayersIds = new int[0]
        };
}
