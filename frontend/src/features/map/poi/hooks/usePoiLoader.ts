import { useEffect } from "react";
import { Feature }   from "ol";
import Point         from "ol/geom/Point";
import VectorSource  from "ol/source/Vector";
import { GeoJSON, WKT } from "ol/format";
import { Style, Fill, Stroke, Circle as CircleStyle } from "ol/style";
import { getGeoFeatures } from "../../core/api/geoService";
import type { PoiResponseDto } from "../types";

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

const geoJsonFormat = new GeoJSON();
const wktFormat     = new WKT();

const CATEGORY_COLORS: Record<number, { fill: string; stroke: string }> = {
  2: { fill: "#f97316", stroke: "#7c2d12" }, // Restoran
  3: { fill: "#92400e", stroke: "#451a03" }, // Kafe
  5: { fill: "#7c3aed", stroke: "#4c1d95" }, // Müze
  7: { fill: "#059669", stroke: "#064e3b" }, // Cami
};
const DEFAULT_CATEGORY_COLOR = { fill: "#6b7280", stroke: "#374151" };

export function buildPoiStyle(categoryId: number | null): Style {
  const colors = (categoryId !== null && CATEGORY_COLORS[categoryId]) || DEFAULT_CATEGORY_COLOR;
  return new Style({
    image: new CircleStyle({
      radius: 7,
      fill:   new Fill({ color: colors.fill }),
      stroke: new Stroke({ color: colors.stroke, width: 2 }),
    }),
  });
}

export const poiMarkerStyle = buildPoiStyle(null);

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
  feature.setStyle(buildPoiStyle(dto.categoryId));
  return feature;
}

export function usePoiLoader(map: unknown, source: VectorSource, apiFetch: ApiFetch) {
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
          f.setStyle(buildPoiStyle(categoryId));

          source.addFeature(f);
        }
      } catch { }
    }

    load();
  }, [map]);
}
