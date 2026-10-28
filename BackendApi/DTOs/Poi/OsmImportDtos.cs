using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Poi;

public sealed record OsmImportRequestDto(
    [Range(-180, 180)] double MinLon,
    [Range(-90, 90)]   double MinLat,
    [Range(-180, 180)] double MaxLon,
    [Range(-90, 90)]   double MaxLat);

// Skipped = existing OSM POIs left untouched because a user edited or deleted them.
// SkippedCategories = mapped category IDs that are missing or deleted; their OSM tags were not imported.
public sealed record OsmImportResultDto(int Added, int Updated, int Skipped, IReadOnlyList<int> SkippedCategories);
