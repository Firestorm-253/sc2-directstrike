﻿using System.Text.Json;

namespace sc2_directstrike.api.DTOs;

public record ReplayPlayer
{
    public ulong Id { get; init; }
    public ulong PlayerId { get; init; }
    public ulong ReplayId { get; init; }

    public uint Team { get; init; }
    public uint Position { get; init; }
    public uint Result { get; init; } // 0-Loss, 1-Win
    public uint Duration { get; init; }

    public string Commander { get; init; } = null!;


    public static implicit operator Dictionary<string, object>(ReplayPlayer replayPlayer)
        => new()
        {
            { "Id", replayPlayer.Id },
            { "PlayerId", replayPlayer.PlayerId },
            { "ReplayId", replayPlayer.ReplayId },

            { "Team", replayPlayer.Team },
            { "Position", replayPlayer.Position },
            { "Result", replayPlayer.Result },
            { "Duration", replayPlayer.Duration },

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
            Result = (uint)entry["Result"],
            Duration = (uint)entry["Duration"],

            Commander = (string)entry["Commander"],
        };
    }
}

public record PostReplayPlayer
{
    public PostPlayer Player { get; init; } = null!;

    public uint Team { get; init; }
    public uint Position { get; init; }
    public uint Result { get; init; }
    public uint Duration { get; init; }

    public string Commander { get; init; } = null!;


    public static implicit operator ReplayPlayer(PostReplayPlayer postReplayPlayer)
        => new()
        {
            Team = postReplayPlayer.Team,
            Position = postReplayPlayer.Position,
            Result = postReplayPlayer.Result,
            Duration = postReplayPlayer.Duration,

            Commander = postReplayPlayer.Commander,
        };
}
