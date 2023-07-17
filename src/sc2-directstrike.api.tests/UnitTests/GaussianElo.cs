namespace sc2_directstrike.api.tests.UnitTests;

using Services.Data.RatingService;
using Services.Data.RatingService.Maths;

public class GaussianEloTests
{
    [Test]
    public void GetRatingAfter()
    {
        var ratingA_before = Gaussian.ByMeanDeviation(1907.11, 1.12995);
        var ratingB_before = Gaussian.ByMeanDeviation(1924.59, 0.0325302);

        var (predictionA, matchDistA) = GaussianElo.PredictMatch(ratingA_before, ratingB_before);
        var (predictionB, matchDistB) = GaussianElo.PredictMatch(ratingB_before, ratingA_before);
        

        var ratingA_afterWin = GaussianElo.GetRatingAfter(ratingA_before, 1, predictionA, matchDistA, 1, false, RatingOptions.Default.Get(2));
        var ratingB_afterLoss = GaussianElo.GetRatingAfter(ratingB_before, 0, predictionB, matchDistB, 1, false, RatingOptions.Default.Get(2));

        Assert.AreEqual(Math.Round(ratingA_afterWin.Mean - ratingA_before.Mean, 4), Math.Round(9.30135455188, 4));
        Assert.AreEqual(Math.Round(ratingB_afterLoss.Mean - ratingB_before.Mean, 4), Math.Round(-0.01352827316694, 4));


        var ratingA_afterLoss = GaussianElo.GetRatingAfter(ratingA_before, 0, predictionA, matchDistA, 1, false, RatingOptions.Default.Get(2));
        var ratingB_afterWin = GaussianElo.GetRatingAfter(ratingB_before, 1, predictionB, matchDistB, 1, false, RatingOptions.Default.Get(2));

        Assert.AreEqual(Math.Round(ratingA_afterLoss.Mean - ratingA_before.Mean, 4), Math.Round(0.0, 4));
        Assert.AreEqual(Math.Round(ratingB_afterWin.Mean - ratingB_before.Mean, 4), Math.Round(-2.2737367544e-13, 4));
    }
}
