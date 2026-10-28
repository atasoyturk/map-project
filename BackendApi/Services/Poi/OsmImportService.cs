using System.Globalization;
using System.Text.Json;
using BackendApi.Data;
using BackendApi.DTOs.Poi;
using BackendApi.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using PoiEntity = BackendApi.Entities.Poi.Poi;

namespace BackendApi.Services.Poi;

// One-off admin import of POIs from OSM via Overpass. Only admins call it (admin_access), and admins are
// exempt from geographic boundaries, so there is deliberately no IGeoPermissionService check here.
public sealed class OsmImportService : IOsmImportService
{
    private const double MaxBboxSizeDegrees    = 0.5;
    private const int    MaxSizeBytes          = 256 * 1024 * 1024;
    private const int    MaxNameLength         = 150;
    private const int    MaxWorkingHoursLength = 200;

    // OSM tag → POI category (IDs from Data/Seed/SeedData.cs). First match wins.
    private static readonly (string Key, string Value, int CategoryId)[] TagMapping =
    [
        ("amenity",  "restaurant",       2),
        ("amenity",  "cafe",             3),
        ("amenity",  "fast_food",        4),
        ("amenity",  "hospital",         6),
        ("amenity",  "pharmacy",         7),
        ("amenity",  "clinic",           8),
        ("amenity",  "doctors",          8),
        ("amenity",  "dentist",          9),
        ("amenity",  "kindergarten",     11),
        ("amenity",  "school",           12),
        ("amenity",  "university",       13),
        ("amenity",  "college",          13),
        ("amenity",  "language_school",  14),
        ("amenity",  "driving_school",   14),
        ("amenity",  "training",         14),
        ("shop",     "supermarket",      16),
        ("shop",     "convenience",      17),
        ("shop",     "greengrocer",      17),
        ("shop",     "mall",             18),
        ("shop",     "bakery",           19),
        ("amenity",  "bank",             21),
        ("amenity",  "atm",              22),
        ("railway",  "station",          24),
        ("station",  "subway",           24),
        ("railway",  "tram_stop",        24),
        ("amenity",  "parking",          25),
        ("amenity",  "fuel",             26),
        ("amenity",  "townhall",         28),
        ("amenity",  "police",           29),
        ("amenity",  "post_office",      30),
        ("amenity",  "place_of_worship", 32), // only with religion=muslim, see MapCategory
        ("leisure",  "sports_centre",    33),
        ("leisure",  "fitness_centre",   34),
    ];

    private static readonly GeometryFactory GeometryFactory = new(new PrecisionModel(), 4326);

    private readonly AppDbContext              _context;
    private readonly IOverpassClient           _overpassClient;
    private readonly OverpassSettings          _settings;
    private readonly ILogger<OsmImportService> _logger;

    public OsmImportService(
        AppDbContext               context,
        IOverpassClient            overpassClient,
        IOptions<OverpassSettings> settings,
        ILogger<OsmImportService>  logger)
    {
        _context        = context;
        _overpassClient = overpassClient;
        _settings       = settings.Value;
        _logger         = logger;
    }

    public async Task<(OsmImportResultDto? Result, string? Error)> ImportAsync(
        OsmImportRequestDto request, int userId, CancellationToken cancellationToken)
    {
        if (request.MinLon >= request.MaxLon || request.MinLat >= request.MaxLat)
            throw new ArgumentException("Geçersiz alan: minimum koordinatlar maksimumdan küçük olmalıdır.");

        if (request.MaxLon - request.MinLon > MaxBboxSizeDegrees || request.MaxLat - request.MinLat > MaxBboxSizeDegrees)
            throw new ArgumentException($"Alan çok büyük: en fazla {MaxBboxSizeDegrees.ToString(CultureInfo.InvariantCulture)}° × {MaxBboxSizeDegrees.ToString(CultureInfo.InvariantCulture)}° olabilir.");

        // Mapped category IDs come from the seed; drop tags whose category no longer exists (or is deleted).
        var mappedIds = TagMapping.Select(m => m.CategoryId).Distinct().ToList();
        var activeIds = await _context.PoiCategories
            .Where(c => mappedIds.Contains(c.Id) && !c.IsDeleted)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);
        var skippedCategories = mappedIds.Except(activeIds).Order().ToList();
        var mapping           = TagMapping.Where(m => activeIds.Contains(m.CategoryId)).ToArray();

        if (mapping.Length == 0)
            return (new OsmImportResultDto(0, 0, 0, skippedCategories), null);

        IReadOnlyList<OsmElement> elements;
        try
        {
            elements = await _overpassClient.RunQueryAsync(BuildQuery(request, mapping), cancellationToken);
        }
        // Overpass errors, remarks (incomplete data), bad JSON and HttpClient timeouts → 502 in the controller.
        catch (Exception ex) when (ex is HttpRequestException or JsonException
                                   || (ex is TaskCanceledException && !cancellationToken.IsCancellationRequested))
        {
            _logger.LogError(ex, "Overpass import failed");
            return (null, "OSM verisi alınamadı, daha sonra tekrar deneyin.");
        }

        var candidates = elements
            .Select(e => (Element: e, CategoryId: MapCategory(e.Tags, mapping), Name: e.Tags.GetValueOrDefault("name")?.Trim()))
            .Where(c => c.CategoryId is not null && !string.IsNullOrEmpty(c.Name))
            .DistinctBy(c => OsmId(c.Element))
            .ToList();

        var osmIds = candidates.Select(c => OsmId(c.Element)).ToList();

        // Includes soft-deleted rows on purpose: a POI the user deleted must not be re-created.
        var existing = await _context.Pois
            .Where(p => p.OsmId != null && osmIds.Contains(p.OsmId))
            .ToDictionaryAsync(p => p.OsmId!, cancellationToken);

        int added = 0, updated = 0, skipped = 0;
        foreach (var (element, categoryId, name) in candidates)
        {
            var point        = GeometryFactory.CreatePoint(new Coordinate(element.Lon, element.Lat));
            var workingHours = element.Tags.GetValueOrDefault("opening_hours") is { Length: <= MaxWorkingHoursLength } hours
                ? hours
                : string.Empty;
            var poiName      = name!.Length > MaxNameLength ? name[..MaxNameLength] : name;

            if (existing.TryGetValue(OsmId(element), out var poi))
            {
                // Edited or deleted by a user → their version wins. The import never sets ModifiedUserId.
                if (poi.IsDeleted || poi.ModifiedUserId is not null)
                {
                    skipped++;
                    continue;
                }

                poi.Name         = poiName;
                poi.WorkingHours = workingHours;
                poi.Geometry     = point;
                poi.CategoryId   = categoryId!.Value;
                poi.ModifiedDate = DateTime.UtcNow;
                updated++;
            }
            else
            {
                _context.Pois.Add(new PoiEntity
                {
                    Name         = poiName,
                    WorkingHours = workingHours,
                    Geometry     = point,
                    CategoryId   = categoryId!.Value,
                    UserId       = userId,
                    OsmId        = OsmId(element),
                });
                added++;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return (new OsmImportResultDto(added, updated, skipped, skippedCategories), null);
    }

    private static string OsmId(OsmElement e) => $"{e.Type}/{e.Id}";

    private static int? MapCategory(IReadOnlyDictionary<string, string> tags, (string Key, string Value, int CategoryId)[] mapping)
    {
        foreach (var (key, value, categoryId) in mapping)
        {
            if (tags.GetValueOrDefault(key) != value) continue;
            if (value == "place_of_worship" && tags.GetValueOrDefault("religion") != "muslim") continue;
            return categoryId;
        }
        return null;
    }

    // Overpass bbox order is (south, west, north, east).
    private string BuildQuery(OsmImportRequestDto r, (string Key, string Value, int CategoryId)[] mapping)
    {
        var bbox = string.Join(",", new[] { r.MinLat, r.MinLon, r.MaxLat, r.MaxLon }
            .Select(v => v.ToString(CultureInfo.InvariantCulture)));

        var filters = mapping
            .GroupBy(m => m.Key)
            .Select(g => $"  nwr[\"{g.Key}\"~\"^({string.Join("|", g.Select(m => m.Value).Distinct())})$\"]({bbox});");

        return $"[out:json][timeout:{_settings.TimeoutSeconds}][maxsize:{MaxSizeBytes}];\n(\n{string.Join("\n", filters)}\n);\nout center tags;";
    }
}
