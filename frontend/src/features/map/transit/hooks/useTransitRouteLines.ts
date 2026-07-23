import { useEffect } from "react";
import Feature       from "ol/Feature";
import VectorSource  from "ol/source/Vector";
import { WKT }        from "ol/format";
import { buildRouteStyle } from "../../../../utils/mapStyle";
import type { TransitRouteResponseDto } from "../types";

const wktFormat = new WKT();

function routeToFeature(route: TransitRouteResponseDto): Feature | null {
  if (!route.routeWktGeometry) return null;

  const geometry = wktFormat.readGeometry(route.routeWktGeometry, {
    dataProjection:    "EPSG:4326",
    featureProjection: "EPSG:3857",
  });

  const feature = new Feature({ geometry });
  feature.setId(route.id);
  feature.set("routeId",    route.id);
  feature.set("routeName",  route.name);
  feature.set("routeColor", route.color);
  feature.setStyle(buildRouteStyle(route.color));

  return feature;
}

export function useTransitRouteLines(source: VectorSource, routes: TransitRouteResponseDto[]) {
  useEffect(() => {
    const currentRouteIds = new Set(routes.map((r) => r.id));

    for (const feature of source.getFeatures()) {
      const id = feature.get("routeId");
      const route = routes.find((r) => r.id === id);
      
      if (!currentRouteIds.has(id) || !route?.routeWktGeometry) {
        source.removeFeature(feature);
      }
    }

    for (const route of routes) {
      const newFeature = routeToFeature(route);
      if (!newFeature) continue;

      const existing = source.getFeatureById(route.id);
      if (existing) source.removeFeature(existing);
      source.addFeature(newFeature);
    }
  }, [source, routes]);
}