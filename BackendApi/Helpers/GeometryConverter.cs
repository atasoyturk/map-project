using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace BackendApi.Helpers;

public static class GeometryConverter
{
    private static readonly WKTReader Reader = new();
    private static readonly WKTWriter Writer = new();

    public static string ToWkt(Geometry geometry) =>
        Writer.Write(geometry);

    public static Geometry FromWkt(string wkt) =>
        Reader.Read(wkt);
}