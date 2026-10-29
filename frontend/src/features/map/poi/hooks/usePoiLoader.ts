import { useEffect } from "react";
import { Feature }   from "ol";
import Point         from "ol/geom/Point";
import VectorSource  from "ol/source/Vector";
import { GeoJSON, WKT } from "ol/format";
import { Style, Fill, Stroke, Text, Circle as CircleStyle } from "ol/style";
import { getGeoFeatures } from "../../core/api/geoService";
import type { PoiResponseDto } from "../types";
import type { ApiFetch } from "../../../../shared/api/apiFetch";

const geoJsonFormat = new GeoJSON();
const wktFormat     = new WKT();

export const poiMarkerStyle = new Style({
  image: new CircleStyle({
    radius: 7,
    fill:   new Fill({ color: "#6b7280" }),
    stroke: new Stroke({ color: "#374151", width: 2 }),
  }),
});

const poiIconStyles = new Map<string, Style>();

// Emoji on a white disc; categories without an icon (own or inherited) get the grey dot.
export function buildPoiStyle(icon: string | null): Style {
  if (!icon) return poiMarkerStyle;

  let style = poiIconStyles.get(icon);
  if (!style) {
    style = new Style({
      image: new CircleStyle({
        radius: 11,
        fill:   new Fill({ color: "#ffffff" }),
        stroke: new Stroke({ color: "#374151", width: 1.5 }),
      }),
      text: new Text({ text: icon, font: "13px sans-serif", offsetY: 1 }),
    });
    poiIconStyles.set(icon, style);
  }
  return style;
}

export function poiToFeature(dto: PoiResponseDto): Feature {
  const geometry = wktFormat.readGeometry(dto.wktGeometry, {
    dataProjection:    "EPSG:4326",
    featureProjection: "EPSG:3857",
  }) as Point;

  const feature = new Feature({ geometry });
  feature.set("poiId",           dto.id);
  feature.set("poiName",         dto.name);
  feature.set("poiWorkingHours", dto.workingHours);
  feature.set("poiCategoryId",   dto.categoryId);
  feature.set("poiUserId",       dto.userId);
  feature.set("poiCreatedDate",  dto.createdDate);
  return feature;
}

// Changing reloadKey replaces the loaded POIs (e.g. after an OSM import).
export function usePoiLoader(map: unknown, source: VectorSource, apiFetch: ApiFetch, reloadKey: number) {
  useEffect(() => {
    if (!map) return;

    async function load() {
      try {
        const res = await getGeoFeatures(apiFetch, "tbl_poi_active");
        if (!res.ok) return;

        const geojson = await res.json();
        const features = geoJsonFormat.readFeatures(geojson, {
          dataProjection:    "EPSG:4326",
          featureProjection: "EPSG:3857",
        });

        source.clear();
        for (const f of features as Feature[]) {
          const props = f.getProperties();

          const rawId = props["Id"] ?? props["id"] ?? f.getId();
          const id    = typeof rawId === "string" && rawId.includes(".")
            ? parseInt(rawId.split(".")[1])
            : rawId;

          const rawCategoryId = props["CategoryId"] ?? props["categoryId"] ?? null;
          const categoryId    = rawCategoryId !== null ? Number(rawCategoryId) : null;

          f.set("poiId",           id);
          f.set("poiName",         props["Name"]         ?? props["name"]         ?? "");
          f.set("poiWorkingHours", props["WorkingHours"] ?? props["workingHours"] ?? "");
          f.set("poiCategoryId",   categoryId);
          f.set("poiUserId",       props["UserId"]        ?? props["userId"]       ?? null);
          f.set("poiCreatedDate",  props["CreatedDate"]   ?? props["createdDate"]  ?? null);

          source.addFeature(f);
        }
      } catch { }
    }

    load();
  }, [map, reloadKey]);
}
