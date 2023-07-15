namespace sc2_directstrike.client.DTOs;

public record Replay(
    ulong Id,
    DateTime GameTime,
    string GameMode,
    uint Duration
);
public record PostReplay(
    DateTime GameTime,
    string GameMode,
    uint Duration,
    PostReplayPlayer[] ReplayPlayers
);

public record ReplayPlayer(
    ulong Id,
    ulong PlayerId,
    ulong ReplayId,
    uint Team,
    uint Position,
    uint Result,
    uint Duration,
    string Commander,
    float RatingBefore,
    float RatingAfter,
    float DeviationBefore,
    float DeviationAfter
);
public record PostReplayPlayer(
    PostPlayer Player,
    uint Team,
    uint Position,
    uint Result,
    uint Duration,
    string Commander
);

public record Player(
    ulong Id,
    ulong InGameId,
    string Name
);

public record PostPlayer(
    ulong InGameId,
    string Name
);