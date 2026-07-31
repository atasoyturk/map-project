using BackendApi.Data;
using BackendApi.DTOs.Transit;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace BackendApi.Services.Transit;

public sealed class TripPlanningService : ITripPlanningService
{
    private const double WalkSpeedKmh          = 5;
    private const double MaxBusSpeedKmh        = 50;
    private const double StartEndWalkRadiusM   = 1000; // A/B noktasından duraklara yürüme mesafesi
    private const double TransferWalkRadiusM   = 300;  // Aktarma için duraklar arası yürüme mesafesi

    private readonly AppDbContext _context;

    public TripPlanningService(AppDbContext context) => _context = context;

    public async Task<TripPlanResultDto?> PlanTripAsync(double startLon, double startLat, double endLon, double endLat)
    {
        var (nodes, edges) = await BuildGraphAsync(startLon, startLat, endLon, endLat);

        var path = RunDijkstra(nodes, edges, "start", "end");
        if (path is null) return null;

        return BuildItinerary(nodes, path);
    }

    /// İki (lon, lat) koordinatı arasındaki gerçek mesafeyi (metre) Haversine formülüyle hesaplar.
    /// Kısa mesafelerde (şehir içi ölçek) düzlem yaklaşıklığından daha doğru, ama tam küresel
    /// hassasiyet gerektirmeyen pratik bir hesaplama — projenin diğer yerlerinde (frontend offset
    /// hesaplaması) kullanılan aynı derece-metre dönüşüm mantığının backend karşılığı.
    private static double HaversineMeters(double lon1, double lat1, double lon2, double lat2)
    {
        const double earthRadiusM = 6_371_000;
        var dLat = DegToRad(lat2 - lat1);
        var dLon = DegToRad(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegToRad(lat1)) * Math.Cos(DegToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusM * c;
    }

    private sealed record GraphNode(string Id, double Lon, double Lat, string DisplayName);

    private sealed record GraphEdge(
        string  ToNodeId,
        double  DurationSeconds,
        double  DistanceMeters,
        string  Type,           // "Walk" | "Bus"
        string  PathWkt,
        int?    RouteId,
        string? RouteName,
        string? RouteColor);

    private async Task<(Dictionary<string, GraphNode> Nodes, Dictionary<string, List<GraphEdge>> Edges)>
        BuildGraphAsync(double startLon, double startLat, double endLon, double endLat)
    {
        var nodes = new Dictionary<string, GraphNode>
        {
            ["start"] = new("start", startLon, startLat, "Başlangıç"),
            ["end"]   = new("end",   endLon,   endLat,   "Bitiş"),
        };
        var edges = new Dictionary<string, List<GraphEdge>>();

        void AddEdge(string fromId, GraphEdge edge)
        {
            if (!edges.TryGetValue(fromId, out var list))
                edges[fromId] = list = new List<GraphEdge>();
            list.Add(edge);
        }

        // Aktif duraklar + bağlı oldukları güzergah bilgisi
        var stops = await (from s in _context.TransitStops
            join r in _context.TransitRoutes on s.TransitRouteId equals r.Id
            where !s.IsDeleted && !r.IsDeleted
            orderby s.TransitRouteId, s.SortOrder
            select new { s.Id, s.Name, s.TransitRouteId, s.SortOrder, s.Geometry, RouteName = r.Name, RouteColor = r.Color })
            .ToListAsync();

        foreach (var stop in stops)
            nodes[$"stop:{stop.Id}"] = new($"stop:{stop.Id}", stop.Geometry.X, stop.Geometry.Y, stop.Name);

        // "Bin" kenarları — aynı güzergahtaki ardışık duraklar arasında, tek yönlü
        foreach (var routeGroup in stops.GroupBy(s => s.TransitRouteId))
        {
            var ordered = routeGroup.OrderBy(s => s.SortOrder).ToList();
            for (var i = 0; i < ordered.Count - 1; i++)
            {
                var from = ordered[i];
                var to   = ordered[i + 1];
                var distanceMeters = HaversineMeters(from.Geometry.X, from.Geometry.Y, to.Geometry.X, to.Geometry.Y);
                var durationSeconds = distanceMeters / ((MaxBusSpeedKmh * 1000) / 3600.0);

                var pathWkt = new LineString([from.Geometry.Coordinate, to.Geometry.Coordinate]).AsText();

                AddEdge($"stop:{from.Id}", new GraphEdge(
                    $"stop:{to.Id}", durationSeconds, distanceMeters, "Bus", pathWkt,
                    from.TransitRouteId, from.RouteName, from.RouteColor));
            }
        }

        // Yürüme kenarları — çift yönlü olduğundan her ikisi de ekleniyor
        void AddWalkEdge(string fromId, double fromLon, double fromLat, string toId, double toLon, double toLat)
        {
            var distanceMeters = HaversineMeters(fromLon, fromLat, toLon, toLat);
            var durationSeconds = distanceMeters / ((WalkSpeedKmh * 1000) / 3600.0);
            var pathWkt = new LineString([new Coordinate(fromLon, fromLat), new Coordinate(toLon, toLat)]).AsText();

            AddEdge(fromId, new GraphEdge(toId, durationSeconds, distanceMeters, "Walk", pathWkt, null, null, null));
        }

        // Başlangıç/Bitiş noktasından yakın duraklara (ve tersi)
        foreach (var stop in stops)
        {
            var distanceFromStart = HaversineMeters(startLon, startLat, stop.Geometry.X, stop.Geometry.Y);
            if (distanceFromStart <= StartEndWalkRadiusM)
                AddWalkEdge("start", startLon, startLat, $"stop:{stop.Id}", stop.Geometry.X, stop.Geometry.Y);

            var distanceToEnd = HaversineMeters(stop.Geometry.X, stop.Geometry.Y, endLon, endLat);
            if (distanceToEnd <= StartEndWalkRadiusM)
                AddWalkEdge($"stop:{stop.Id}", stop.Geometry.X, stop.Geometry.Y, "end", endLon, endLat);
        }

        // Aktarma — farklı güzergahların birbirine yakın durakları arasında
        for (var i = 0; i < stops.Count; i++)
        {
            for (var j = 0; j < stops.Count; j++)
            {
                if (i == j || stops[i].TransitRouteId == stops[j].TransitRouteId) continue;

                var distance = HaversineMeters(
                    stops[i].Geometry.X, stops[i].Geometry.Y, stops[j].Geometry.X, stops[j].Geometry.Y);

                if (distance <= TransferWalkRadiusM)
                    AddWalkEdge($"stop:{stops[i].Id}", stops[i].Geometry.X, stops[i].Geometry.Y,
                                $"stop:{stops[j].Id}", stops[j].Geometry.X, stops[j].Geometry.Y);
            }
        }

        // Doğrudan Başlangıç → Bitiş yürüyüşü — otobüs kullanmak daha yavaşsa algoritma bunu seçebilsin
        AddWalkEdge("start", startLon, startLat, "end", endLon, endLat);

        return (nodes, edges);
    }

    /// Standart Dijkstra — BCL'nin PriorityQueue'suyla (min-heap), her düğüme olan en kısa
    /// (en az süreli) yolu bulur. Döndürülen liste, "start"tan "end"e giden düğüm-kenar
    /// çiftlerinin sırasıdır; ulaşılamıyorsa null döner.
    private static List<(GraphNode Node, GraphEdge? IncomingEdge)>? RunDijkstra(
        Dictionary<string, GraphNode> nodes,
        Dictionary<string, List<GraphEdge>> edges,
        string startId, string endId)
    {
        var distances    = new Dictionary<string, double> { [startId] = 0 };
        var predecessors = new Dictionary<string, (string FromId, GraphEdge Edge)>();
        var visited      = new HashSet<string>();

        var queue = new PriorityQueue<string, double>();
        queue.Enqueue(startId, 0);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            if (!visited.Add(currentId)) continue;
            if (currentId == endId) break;

            if (!edges.TryGetValue(currentId, out var outgoing)) continue;

            var currentDistance = distances[currentId];

            foreach (var edge in outgoing)
            {
                var newDistance = currentDistance + edge.DurationSeconds;
                if (!distances.TryGetValue(edge.ToNodeId, out var existing) || newDistance < existing)
                {
                    distances[edge.ToNodeId]    = newDistance;
                    predecessors[edge.ToNodeId] = (currentId, edge);
                    queue.Enqueue(edge.ToNodeId, newDistance);
                }
            }
        }

        if (!distances.ContainsKey(endId)) return null;

        // Geriye doğru iz sürerek yolu yeniden kur
        var reversePath = new List<(GraphNode Node, GraphEdge? IncomingEdge)>();
        var cursor = endId;

        while (true)
        {
            if (predecessors.TryGetValue(cursor, out var pred))
            {
                reversePath.Add((nodes[cursor], pred.Edge));
                cursor = pred.FromId;
            }
            else
            {
                reversePath.Add((nodes[cursor], null)); // başlangıç düğümü
                break;
            }
        }

        reversePath.Reverse();
        return reversePath;
    }

    /// Düğüm-kenar zincirini, ardışık aynı-güzergah "Bin" kenarlarını tek bir bacakta
    /// birleştirerek kullanıcıya gösterilecek Yürü/Bin bacak listesine dönüştürür.
    private static TripPlanResultDto BuildItinerary(
        Dictionary<string, GraphNode> nodes,
        List<(GraphNode Node, GraphEdge? IncomingEdge)> path)
    {
        var legs = new List<TripLegDto>();
        var i = 1; // path[0] başlangıç düğümü, kenarı yok

        while (i < path.Count)
        {
            var edge = path[i].IncomingEdge!;
            var fromNode = path[i - 1].Node;

            if (edge.Type == "Bus")
            {
                var segmentEnd = i;
                var totalDuration = 0.0;
                var totalDistance = 0.0;
                var coordinates   = new List<Coordinate> { new(fromNode.Lon, fromNode.Lat) };

                while (segmentEnd < path.Count &&
                       path[segmentEnd].IncomingEdge is { Type: "Bus" } busEdge &&
                       busEdge.RouteId == edge.RouteId)
                {
                    totalDuration += busEdge.DurationSeconds;
                    totalDistance += busEdge.DistanceMeters;
                    coordinates.Add(new Coordinate(path[segmentEnd].Node.Lon, path[segmentEnd].Node.Lat));
                    segmentEnd++;
                }

                legs.Add(new TripLegDto(
                    "Bus", totalDuration, totalDistance,
                    fromNode.DisplayName, path[segmentEnd - 1].Node.DisplayName,
                    new LineString(coordinates.ToArray()).AsText(),
                    edge.RouteId, edge.RouteName, edge.RouteColor));

                i = segmentEnd;
            }
            else
            {
                legs.Add(new TripLegDto(
                    "Walk", edge.DurationSeconds, edge.DistanceMeters,
                    fromNode.DisplayName, path[i].Node.DisplayName,
                    edge.PathWkt, null, null, null));
                i++;
            }
        }

        return new TripPlanResultDto(
            legs.Sum(l => l.DurationSeconds),
            legs.Where(l => l.Type == "Walk").Sum(l => l.DistanceMeters),
            legs);
    }

    private static double DegToRad(double deg) => deg * Math.PI / 180;
}