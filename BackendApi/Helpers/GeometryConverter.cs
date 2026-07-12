using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace BackendApi.Helpers;

public static class GeometryConverter
{
    private static readonly WKTReader Reader = new();
    private static readonly WKTWriter Writer = new();

    private const int MaxCoordinateCount = 10_000; // Spatial DoS guard

    public static string ToWkt(Geometry geometry) =>
        Writer.Write(geometry);

    public static Geometry FromWkt(string wkt)
    {
        var geometry = Reader.Read(wkt);

        if (geometry is null || geometry.IsEmpty)
            throw new ArgumentException("WKT boş veya geçersiz geometri.");

        if (geometry.NumPoints > MaxCoordinateCount)
            throw new ArgumentException(
                $"Geometri çok fazla nokta içeriyor (max {MaxCoordinateCount}).");

        return geometry;
    }
}