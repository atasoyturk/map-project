import { useEffect, useRef } from "react";
import type OlMap    from "ol/Map";
import Feature        from "ol/Feature";
import Point           from "ol/geom/Point";
import VectorSource    from "ol/source/Vector";
import VectorLayer     from "ol/layer/Vector";
import { WKT }          from "ol/format";
import { fromLonLat }   from "ol/proj";
import { Style, Stroke, Circle as CircleStyle, Fill } from "ol/style";
import type { TripPlanResultDto } from "../../../../shared/api/tripPlanningService";

const wktFormat = new WKT();

function buildLegStyle(type: "Walk" | "Bus", color: string | null): Style {
  return new Style({
    stroke: new Stroke({
      color:     type === "Bus" ? (color ?? "#3b82f6") : "#64748b",
      width:     type === "Bus" ? 5 : 3,
      lineDash:  type === "Walk" ? [6, 6] : undefined,
    }),
  });
}

function buildPointStyle(color: string): Style {
  return new Style({
    image: new CircleStyle({
      radius: 8,
      fill:   new Fill({ color }),
      stroke: new Stroke({ color: "#ffffff", width: 2 }),
    }),
  });
}

interface TripPoint { lon: number; lat: number; }

export function useTripRouteLayer(
  map: OlMap | null,
  result: TripPlanResultDto | null,
  startPoint: TripPoint | null,
  endPoint: TripPoint | null,
) {
  const sourceRef = useRef(new VectorSource());

  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({ source: sourceRef.current, zIndex: 150, updateWhileAnimating: true, updateWhileInteracting: true });
    map.addLayer(layer);
    return () => { map.removeLayer(layer); };
  }, [map]);

  useEffect(() => {
    const source = sourceRef.current;
    source.clear();

    if (startPoint) {
      const f = new Feature({ geometry: new Point(fromLonLat([startPoint.lon, startPoint.lat])) });
      f.setStyle(buildPointStyle("#22c55e"));
      source.addFeature(f);
    }

    if (endPoint) {
      const f = new Feature({ geometry: new Point(fromLonLat([endPoint.lon, endPoint.lat])) });
      f.setStyle(buildPointStyle("#ef4444"));
      source.addFeature(f);
    }

    if (!result) return;

    for (const leg of result.legs) {
      const geometry = wktFormat.readGeometry(leg.pathWkt, {
        dataProjection:    "EPSG:4326",
        featureProjection: "EPSG:3857",
      });
      const feature = new Feature({ geometry });
      feature.setStyle(buildLegStyle(leg.type, leg.routeColor));
      source.addFeature(feature);
    }
  }, [result, startPoint, endPoint]);
}