namespace sc2_directstrike.api.Services;
using Data.RatingService.Maths;
using Data.RatingService.ProcessData;
using Data.RatingService;

public partial class RatingService
{
    public void SetReplayData(ReplayData replayData,
                              RatingOptions ratingOptions)
    {
        SetTeamData(replayData.Team1, replayData, ratingOptions);
        SetTeamData(replayData.Team2, replayData, ratingOptions);

        SetExpectationsToWin(replayData, ratingOptions);

        replayData.Confidence = Gaussian.GetPrecision(replayData.Team1.Deviation + replayData.Team2.Deviation);
    }

    private void SetTeamData(TeamData teamData, 
                             ReplayData replayData,
                             RatingOptions ratingOptions)
    {
        Gaussian summedDistributions = Gaussian.ByMeanDeviation(0, 0);
        foreach (var playerData in teamData.Players)
        {
            SetPlayerData(playerData, replayData, ratingOptions);

            summedDistributions += playerData.Distribution;
        }

        teamData.Distribution = Gaussian.ByMeanDeviation(summedDistributions.Mean, summedDistributions.Deviation);
    }

    private void SetExpectationsToWin(ReplayData replayData,
                                      RatingOptions ratingOptions)
    {
        var (expectationToWin_team1, match) = GaussianElo.PredictMatch(replayData.Team1.Distribution,
                                                                       replayData.Team2.Distribution,
                                                                       ratingOptions.BalanceDeviationOffset);
        
        replayData.Team1.Prediction = match;
        replayData.Team2.Prediction = Gaussian.ByMeanDeviation(-match.Mean, match.Deviation);

        replayData.Team1.ExpectedResult = expectationToWin_team1;
        replayData.Team2.ExpectedResult = (1 - expectationToWin_team1);
    }

    private void SetPlayerData(PlayerData playerData,
                               ReplayData replayData,
                               RatingOptions ratingOptions)
    {
        var replayPlayers = playerRatings[replayData.Replay.GameMode][playerData.ReplayPlayer.PlayerId];

        if (replayPlayers.Any())
        {
            var lastReplayPlayer = replayPlayers.Last();
            var lastReplayPlayer_Replay = replays[lastReplayPlayer.ReplayId];

            playerData.TimeSinceLastGame = replayData.Replay.GameTime - lastReplayPlayer_Replay.GameTime;
            playerData.Rating = lastReplayPlayer.RatingAfter;
            playerData.Deviation = lastReplayPlayer.DeviationAfter;
        }
        else
        {
            playerData.TimeSinceLastGame = new TimeSpan(0);
            playerData.Rating = ratingOptions.StartRating;
            playerData.Deviation = ratingOptions.StandardPlayerDeviation;
        }

        var decayFactor = GetDecayFactor(playerData.TimeSinceLastGame, ratingOptions);
        playerData.Deviation = (float)Math.Min(ratingOptions.StandardPlayerDeviation, playerData.Deviation * decayFactor);

        playerData.ReplayPlayer.RatingBefore = playerData.Rating;
        playerData.ReplayPlayer.DeviationBefore = playerData.Deviation;

        playerRatings[replayData.Replay.GameMode][playerData.ReplayPlayer.PlayerId].Add(playerData.ReplayPlayer);
    }
}
