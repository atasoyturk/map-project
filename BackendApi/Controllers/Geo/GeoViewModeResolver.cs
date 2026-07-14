namespace BackendApi.Controllers.Geo;

internal static class GeoViewModeResolver
{
    public static Services.Geo.GeoViewMode Resolve(bool hasAdminAccess, int? teamId, string? requestedMode)
    {
        if (hasAdminAccess)
            return Services.Geo.GeoViewMode.All;

        if (string.Equals(requestedMode, "team", StringComparison.OrdinalIgnoreCase) && teamId is not null)
            return Services.Geo.GeoViewMode.Team;

        return Services.Geo.GeoViewMode.Own;
    }
}