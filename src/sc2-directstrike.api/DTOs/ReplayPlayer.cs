using System.Text.Json;

namespace sc2_directstrike.api.DTOs;

public record ReplayPlayer
{
    public ulong Id { get; init; }
    public ulong PlayerId { get; init; }
    public ulong ReplayId { get; init; }

    public uint Team { get; init; }
    public uint Position { get; init; }
    public uint Duration { get; init; }

    public float RatingBefore { get; set; }
    public float RatingAfter { get; set; }

    public string Commander { get; init; } = null!;


    public static implicit operator Dictionary<string, object>(ReplayPlayer replayPlayer)
        => new()
        {
            { "Id", replayPlayer.Id },
            { "PlayerId", replayPlayer.PlayerId },
            { "ReplayId", replayPlayer.ReplayId },

            { "Team", replayPlayer.Team },
            { "Position", replayPlayer.Position },
            { "Duration", replayPlayer.Duration },

            { "RatingBefore", replayPlayer.RatingBefore },
            { "RatingAfter", replayPlayer.RatingAfter },

            { "Commander", replayPlayer.Commander },
        };

    public static implicit operator ReplayPlayer?(Dictionary<string, object> entry)
    {
        if (entry == null)
        {
            return null;
        }

        return new ReplayPlayer()
        {
            Id = (ulong)entry["Id"],
            PlayerId = (ulong)entry["PlayerId"],
            ReplayId = (ulong)entry["ReplayId"],

            Team = (uint)entry["Team"],
            Position = (uint)entry["Position"],
            Duration = (uint)entry["Duration"],

            RatingBefore = (float)entry["RatingBefore"],
            RatingAfter = (float)entry["RatingAfter"],

            Commander = (string)entry["Commander"],
        };
    }
}

public record PostReplayPlayer
{
    public PostPlayer Player { get; init; } = null!;

    public uint Team { get; init; }
    public uint Position { get; init; }

    public string Commander { get; init; } = null!;


    public static implicit operator ReplayPlayer(PostReplayPlayer postReplayPlayer)
        => new()
        {
            Team = postReplayPlayer.Team,
            Position = postReplayPlayer.Position,

            Commander = postReplayPlayer.Commander,
        };
}
