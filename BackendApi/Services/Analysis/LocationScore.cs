using BackendApi.DTOs.Analysis;

namespace BackendApi.Services.Analysis;

// Explainable 0–100 location score: a weighted sum of three 0–100 sub-scores.
// The score is relative (for comparing candidates), not a calibrated suitability threshold.
public static class LocationScore
{
    public const double ComplementWeight  = 0.40;
    public const double CompetitionWeight = 0.35;
    public const double TransitWeight     = 0.25;

    // Distance decay by driving band: complements within 5 min count fully, 5–10 min half, 10–15 min a quarter.
    public const double Within5Weight  = 1.0;
    public const double Within10Weight = 0.5;
    public const double Within15Weight = 0.25;
    // Weighted complement count that earns the full complement score; log scale below it.
    public const double ComplementSaturation = 30;

    // Competitors per complement in the 10-min band at which the competition score is 50.
    public const double ReferenceSaturationRatio = 0.5;

    public const double FullScoreStopMeters = 300;
    public const double ZeroScoreStopMeters = 1000;
    public const int    FullScoreStopCount  = 20;

    // Counts are cumulative per band (within5 ⊆ within10 ⊆ within15).
    public static int ComplementScore(int within5, int within10, int within15)
    {
        var weighted = within5 * Within5Weight
                     + (within10 - within5)  * Within10Weight
                     + (within15 - within10) * Within15Weight;

        return Round(100 * Math.Min(1, Math.Log(1 + weighted) / Math.Log(1 + ComplementSaturation)));
    }

    // Competition relative to demand: competitors per complement. Without complements there is no demand
    // signal, so zero competitors there scores 0, not 100.
    public static int CompetitionScore(int competitors10, int complements10)
    {
        if (complements10 == 0) return 0;

        var ratio = (double)competitors10 / complements10;
        return Round(100 / (1 + ratio / ReferenceSaturationRatio));
    }

    // Half nearest-stop distance (full at ≤300 m, zero at ≥1000 m, linear between), half stop count in 10 min.
    public static int TransitScore(double? nearestStopMeters, int stops10)
    {
        var distancePart = nearestStopMeters is { } m
            ? Math.Clamp((ZeroScoreStopMeters - m) / (ZeroScoreStopMeters - FullScoreStopMeters), 0, 1)
            : 0;
        var countPart = Math.Min(1, (double)stops10 / FullScoreStopCount);

        return Round(100 * (distancePart + countPart) / 2);
    }

    public static int Total(int complementScore, int competitionScore, int transitScore)
        => Round(complementScore * ComplementWeight + competitionScore * CompetitionWeight + transitScore * TransitWeight);

    public static LocationScoreDto Compute(
        int complements5, int complements10, int complements15, int competitors10, double? nearestStopMeters, int stops10)
    {
        var complement  = ComplementScore(complements5, complements10, complements15);
        var competition = CompetitionScore(competitors10, complements10);
        var transit     = TransitScore(nearestStopMeters, stops10);

        return new LocationScoreDto(
            Total(complement, competition, transit), complement, competition, transit,
            ComplementWeight, CompetitionWeight, TransitWeight,
            complements10 == 0 ? null : (double)competitors10 / complements10);
    }

    private static int Round(double value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);
}
