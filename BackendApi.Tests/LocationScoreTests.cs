using BackendApi.Services.Analysis;

namespace BackendApi.Tests;

// Pins the location score formula; no database needed.
public sealed class LocationScoreTests
{
    [Fact]
    public void Weights_SumToOne()
        => Assert.Equal(1.0, LocationScore.ComplementWeight + LocationScore.CompetitionWeight + LocationScore.TransitWeight, 10);

    [Theory]
    [InlineData(0,   0,   0,   0)]
    [InlineData(1,   2,   3,   29)]  // 1 + 0.5 + 0.25 = 1.75 weighted → ln(2.75)/ln(31)
    [InlineData(0,   0,   4,   20)]  // farther complements count less: 4 × 0.25 = 1 weighted
    [InlineData(1,   1,   1,   20)]  // the same single complement within 5 min: 1 weighted
    [InlineData(30,  30,  30,  100)] // saturation reached
    [InlineData(100, 200, 300, 100)] // capped
    public void ComplementScore_WeightsBandsAndSaturates(int within5, int within10, int within15, int expected)
        => Assert.Equal(expected, LocationScore.ComplementScore(within5, within10, within15));

    [Theory]
    [InlineData(0, 0,  0)]   // no complements: no demand signal, zero competitors is not the best score
    [InlineData(5, 0,  0)]
    [InlineData(0, 4,  100)] // demand and no competitors
    [InlineData(1, 2,  50)]  // reference ratio 0.5
    [InlineData(2, 2,  33)]  // ratio 1
    [InlineData(2, 20, 83)]  // same competitors, more demand → higher
    [InlineData(8, 2,  11)]  // saturated
    public void CompetitionScore_IsRelativeToComplements(int competitors, int complements, int expected)
        => Assert.Equal(expected, LocationScore.CompetitionScore(competitors, complements));

    [Theory]
    [InlineData(null,   0,  0)]   // no stops at all
    [InlineData(200.0,  0,  50)]  // within full-score distance, no stops in the band
    [InlineData(300.0,  20, 100)]
    [InlineData(650.0,  0,  25)]  // halfway between 300 m and 1000 m
    [InlineData(1000.0, 10, 25)]
    [InlineData(5000.0, 40, 50)]  // count capped at 20
    public void TransitScore_CombinesNearestDistanceAndCount(double? nearestMeters, int stops, int expected)
        => Assert.Equal(expected, LocationScore.TransitScore(nearestMeters, stops));

    [Theory]
    [InlineData(100, 100, 100, 100)]
    [InlineData(0,   0,   0,   0)]
    [InlineData(29,  33,  53,  36)]  // 11.6 + 11.55 + 13.25
    [InlineData(100, 0,   0,   40)]
    [InlineData(0,   100, 0,   35)]
    [InlineData(0,   0,   100, 25)]
    public void Total_IsWeightedSumOfSubScores(int complement, int competition, int transit, int expected)
        => Assert.Equal(expected, LocationScore.Total(complement, competition, transit));

    [Fact]
    public void Compute_UsesTenMinuteBandForSaturationRatio()
    {
        var score = LocationScore.Compute(
            complements5: 1, complements10: 2, complements15: 3, competitors10: 2, nearestStopMeters: 200, stops10: 1);

        Assert.Equal((29, 33, 53, 36), (score.ComplementScore, score.CompetitionScore, score.TransitScore, score.Total));
        Assert.Equal(1.0, score.SaturationRatio);
    }

    [Fact]
    public void Compute_NoComplements_HasNoSaturationRatio()
        => Assert.Null(LocationScore.Compute(0, 0, 0, 3, null, 0).SaturationRatio);
}
