import { useEffect } from "react";
import type OlMap    from "ol/Map";
import Feature       from "ol/Feature";
import LineString    from "ol/geom/LineString";
import VectorSource  from "ol/source/Vector";
import { WKT }        from "ol/format";
import lineOffset     from "@turf/line-offset";
import { buildRouteStyle } from "../../../../shared/utils/mapStyle";
import type { TransitRouteResponseDto } from "../../../../shared/api/transitTypes";

const wktFormat = new WKT();


const OFFSET_STEP_PIXELS = 6;
const MAX_PLAUSIBLE_DEVIATION_METERS = 2000;

function offsetIndexFor(index: number): number {
  if (index === 0) return 0;
  const magnitude = Math.ceil(index / 2);
  return index % 2 === 1 ? magnitude : -magnitude;
}

function sanitizeOffsetResult(original: number[][], offset: number[][]): number[][] {
  if (offset.length !== original.length) return original;

  return offset.map((point, i) => {
    const origPoint = original[i];
    if (!point || point.length !== 2) return origPoint;

    const [lon, lat] = point;
    if (!Number.isFinite(lon) || !Number.isFinite(lat)) return origPoint;
    if (lon < -180 || lon > 180 || lat < -90 || lat > 90) return origPoint;

    const [origLon, origLat] = origPoint;
    const dLon = (lon - origLon) * 111320 * Math.cos((origLat * Math.PI) / 180);
    const dLat = (lat - origLat) * 110540;
    const deviationMeters = Math.sqrt(dLon * dLon + dLat * dLat);

    return deviationMeters > MAX_PLAUSIBLE_DEVIATION_METERS ? origPoint : point;
  });
}

function routeToFeature(route: TransitRouteResponseDto, offsetIndex: number, offsetMeters: number): Feature | null {
  if (!route.routeWktGeometry) return null;

  const geometry4326 = wktFormat.readGeometry(route.routeWktGeometry, {
    dataProjection:    "EPSG:4326",
    featureProjection: "EPSG:4326",
  }) as LineString;

  let coordinates = geometry4326.getCoordinates();
  const totalOffsetMeters = offsetIndex * offsetMeters;

  if (totalOffsetMeters !== 0) {
    try {
      const offsetResult = lineOffset(
        { type: "LineString", coordinates },
        totalOffsetMeters,
        { units: "meters" },
      );
      coordinates = sanitizeOffsetResult(coordinates, offsetResult.geometry.coordinates as number[][]);
    } catch {
      // Ofset hesaplanamazsa (örn. dejenere/çok kısa geometri), orijinal koordinatlarla devam edilir.
    }
  }

  const geometry = new LineString(coordinates);
  geometry.transform("EPSG:4326", "EPSG:3857");

  const feature = new Feature({ geometry });
  feature.setId(route.id);
  feature.set("routeId",    route.id);
  feature.set("routeName",  route.name);
  feature.set("routeColor", route.color);
  feature.setStyle(buildRouteStyle(route.color));

  return feature;
}

function renderRoutes(source: VectorSource, routes: TransitRouteResponseDto[], offsetMeters: number) {
  const currentRouteIds = new Set(routes.map((r) => r.id));

  for (const feature of source.getFeatures()) {
    const id = feature.get("routeId");
    const route = routes.find((r) => r.id === id);
    if (!currentRouteIds.has(id) || !route?.routeWktGeometry) {
      source.removeFeature(feature);
    }
  }

  const routesWithGeometry = routes
    .filter((r) => r.routeWktGeometry)
    .sort((a, b) => a.id - b.id);

  routesWithGeometry.forEach((route, index) => {
    const newFeature = routeToFeature(route, offsetIndexFor(index), offsetMeters);
    if (!newFeature) return;

    const existing = source.getFeatureById(route.id);
    if (existing) source.removeFeature(existing);
    source.addFeature(newFeature);
  });
}

export function useTransitRouteLines(map: OlMap | null, source: VectorSource, routes: TransitRouteResponseDto[]) {
  useEffect(() => {
    if (!map) return;

    const view = map.getView();

    function render() {
      const resolution = view.getResolution() ?? 1;
      // EPSG:3857 metre/piksel değeri doğrudan resolution'dır (Web Mercator projeksiyonunda).
      const offsetMeters = OFFSET_STEP_PIXELS * resolution;
      renderRoutes(source, routes, offsetMeters);
    }

    render();

    view.on("change:resolution", render);
    return () => { view.un("change:resolution", render); };
  }, [map, source, routes]);
}