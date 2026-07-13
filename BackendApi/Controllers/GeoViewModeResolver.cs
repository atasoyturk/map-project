using BackendApi.Services.Geo;

namespace BackendApi.Controllers;

internal static class GeoViewModeResolver
{
    /// <summary>
    /// Admin her zaman All. Diğerleri, ?viewMode=team istenmişse VE bir takıma bağlıysa Team,
    /// aksi halde Own (varsayılan, mevcut davranış).
    /// </summary>
    public static GeoViewMode Resolve(IEnumerable<string> roles, int? teamId, string? requestedMode)
    {
        if (roles.Contains("Admin"))
            return GeoViewMode.All;

        if (string.Equals(requestedMode, "team", StringComparison.OrdinalIgnoreCase) && teamId is not null)
            return GeoViewMode.Team;

        return GeoViewMode.Own;
    }
}