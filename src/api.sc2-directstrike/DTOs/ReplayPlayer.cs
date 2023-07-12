using System.Text.Json;

namespace api.sc2_directstrike.DTOs;

public record ReplayPlayer
{
    public uint Id { get; init; }
    public uint PlayerId { get; init; }
    public uint ReplayId { get; init; }

    public uint Team { get; init; }
    public uint Position { get; init; }

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
            Id = (uint)entry["Id"],
            PlayerId = (uint)entry["PlayerId"],
            ReplayId = (uint)entry["ReplayId"],

            Team = (uint)entry["Team"],
            Position = (uint)entry["Position"],

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
