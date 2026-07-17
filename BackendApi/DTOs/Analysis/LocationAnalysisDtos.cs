namespace BackendApi.DTOs.Analysis;

public sealed class LocationAnalysisCriterionDto
{
    public int CategoryId { get; init; }
    public int Score { get; init; } // 100 üzerinden puan
}

public sealed class LocationAnalysisRequestDto
{
    public string WktGeometry { get; init; } = string.Empty;
    public List<LocationAnalysisCriterionDto> Criteria { get; init; } = new();
}

public sealed class HeatmapPointDto
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double Weight { get; init; } // 0-1 arası normalize edilmiş ağırlık
}

public sealed class LocationAnalysisResponseDto
{
    public List<HeatmapPointDto> HeatmapPoints { get; init; } = new();
}
