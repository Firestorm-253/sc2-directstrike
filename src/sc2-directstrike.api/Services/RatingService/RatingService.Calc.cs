namespace sc2_directstrike.api.Services;
using Data.RatingService.Maths;
using Data.RatingService.ProcessData;
using Data.RatingService;

public partial class RatingService
{
    private void CalculateRatingsDeltas(TeamData teamData,
                                        ReplayData replayData,
                                        RatingOptions ratingOptions)
    {
        foreach (var playerData in teamData.Players)
        {
            if (!playerData.IsLeaver)
            {
                SetPlayerDeltas(playerData, teamData, replayData, ratingOptions);
            }
            else
            {
                SetLeaverPlayerDeltas(playerData, teamData, replayData, ratingOptions);
            }
        }
    }

    private void SetPlayerDeltas(PlayerData playerData,
                                 TeamData teamData,
                                 ReplayData replayData,
                                 RatingOptions ratingOptions)
    {
        var ratingAfter = GaussianElo.GetRatingAfter(
            playerData.Distribution,
            teamData.ActualResult,
            teamData.ExpectedResult,
            teamData.Prediction,
            1,
            playerData.IsLeaver,
            ratingOptions
            );

        playerData.ReplayPlayerRating.RatingAfter = (float)ratingAfter.Mean;
        playerData.ReplayPlayerRating.DeviationAfter = (float)ratingAfter.Deviation;

        playerData.Deltas.Rating = (float)ratingAfter.Mean - playerData.Rating;
        playerData.Deltas.Deviation = (float)ratingAfter.Deviation - playerData.Deviation;
    }

    private void SetLeaverPlayerDeltas(PlayerData playerData,
                                       TeamData teamData,
                                       ReplayData replayData,
                                       RatingOptions ratingOptions)
    {
        var ratingAfter = GaussianElo.GetRatingAfter(
            playerData.Distribution,
            0,
            teamData.ExpectedResult,
            teamData.Prediction,
            1,
            playerData.IsLeaver,
            ratingOptions
            );

        playerData.ReplayPlayerRating.RatingAfter = (float)ratingAfter.Mean;
        playerData.ReplayPlayerRating.DeviationAfter = (float)ratingAfter.Deviation;

        playerData.Deltas.Rating = (float)ratingAfter.Mean - playerData.Rating;
        playerData.Deltas.Deviation = (float)ratingAfter.Deviation - playerData.Deviation;
    }
}
