import { useEffect } from "react";
import { Feature }   from "ol";
import VectorSource  from "ol/source/Vector";
import { GeoJSON }   from "ol/format";
import { buildStyle } from "../../../../utils/mapStyle";
import { getGeoFeatures } from "../../core/api/geoService";

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

const geoJsonFormat = new GeoJSON();

export function useTransitStopLoader(map: unknown, source: VectorSource, apiFetch: ApiFetch) {
  useEffect(() => {
    if (!map) return;

    async function load() {
      try {
        const res = await getGeoFeatures(apiFetch, "tbl_transit_stop_active");
        if (!res.ok) return;

        const geojson = await res.json();
        const features = geoJsonFormat.readFeatures(geojson, {
          dataProjection:    "EPSG:4326",
          featureProjection: "EPSG:3857",
        });

        for (const f of features as Feature[]) {
          const props = f.getProperties();

          const rawId = props["Id"] ?? props["id"] ?? f.getId();
          const id    = typeof rawId === "string" && rawId.includes(".")
            ? parseInt(rawId.split(".")[1])
            : rawId;

          const name      = props["Name"]        ?? props["name"]        ?? "";
          const routeId   = props["TransitRouteId"] ?? props["transitRouteId"] ?? null;
          const routeName = props["RouteName"]   ?? props["routeName"]   ?? "";
          const routeColor= props["RouteColor"]  ?? props["routeColor"]  ?? "#3b82f6";

          f.set("stopId",        id);
          f.set("stopName",      name);
          f.set("stopRouteId",   routeId !== null ? Number(routeId) : null);
          f.set("stopRouteName", routeName);
          f.set("stopUserId",    props["UserId"]      ?? props["userId"]      ?? null);
          f.set("stopCreatedDate", props["CreatedDate"] ?? props["createdDate"] ?? null);
          f.setStyle(buildStyle(routeColor, name));

          source.addFeature(f);
        }
      } catch { }
    }

    load();
  }, [map]);
}