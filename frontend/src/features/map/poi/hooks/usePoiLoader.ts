import { useEffect } from "react";
import { Feature }   from "ol";
import Point         from "ol/geom/Point";
import VectorSource  from "ol/source/Vector";
import { GeoJSON, WKT } from "ol/format";
import { Style, Fill, Stroke, Circle as CircleStyle, Icon } from "ol/style";
import { getGeoFeatures } from "../../core/api/geoService";
import type { PoiResponseDto } from "../types";
import type { ApiFetch } from "../../../../shared/api/apiFetch";

const geoJsonFormat = new GeoJSON();
const wktFormat     = new WKT();

const FOOD_CATEGORY_IDS    = new Set([1, 2, 3, 9, 20]);   // Yeme İçme
const TOURISM_CATEGORY_IDS = new Set([4, 5, 12, 13, 14]); // Turizm
const WORSHIP_CATEGORY_IDS = new Set([6, 7, 11]);         // İbadet
const HEALTH_CATEGORY_IDS  = new Set([15, 16, 17, 18, 19]); // Sağlık

const FOOD_ICON_SVG = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24"><circle cx="12" cy="12" r="11" fill="%23f97316" stroke="%237c2d12" stroke-width="1.5"/><g stroke="%23ffffff" stroke-width="1.4" fill="none" stroke-linecap="round" stroke-linejoin="round"><path d="M8 6v5a1.5 1.5 0 0 0 3 0V6"/><path d="M9.5 6v12"/><path d="M15.5 6c-1 0-1.5 1-1.5 2.5S14.5 12 15.5 12"/><path d="M15.5 12v6"/></g></svg>';

const TOURISM_ICON_SVG = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24"><circle cx="12" cy="12" r="11" fill="%237c3aed" stroke="%234c1d95" stroke-width="1.5"/><g stroke="%23ffffff" stroke-width="1.3" fill="none" stroke-linecap="round" stroke-linejoin="round"><path d="M6 18.5h12"/><path d="M6.5 9.5h11"/><path d="M12 5l6 4.5H6z"/><path d="M8 9.5v7"/><path d="M12 9.5v7"/><path d="M16 9.5v7"/></g></svg>';

const WORSHIP_ICON_SVG = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24"><circle cx="12" cy="12" r="11" fill="%23059669" stroke="%23064e3b" stroke-width="1.5"/><g stroke="%23ffffff" stroke-width="1.3" fill="none" stroke-linecap="round" stroke-linejoin="round"><path d="M6 18.5h12"/><path d="M7 18.5v-6a5 5 0 0 1 10 0v6"/><path d="M12 5.5v2"/><path d="M11 5h2"/><path d="M9.5 18.5v-3.5a2.5 2.5 0 0 1 5 0v3.5"/></g></svg>';

const HEALTH_ICON_SVG = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24"><circle cx="12" cy="12" r="11" fill="%23dc2626" stroke="%237f1d1d" stroke-width="1.5"/><g fill="%23ffffff"><rect x="10.3" y="6" width="3.4" height="12" rx="1"/><rect x="6" y="10.3" width="12" height="3.4" rx="1"/></g></svg>';

function getIconSrc(categoryId: number | null): string | null {
  if (categoryId === null) return null;
  if (FOOD_CATEGORY_IDS.has(categoryId))    return FOOD_ICON_SVG;
  if (TOURISM_CATEGORY_IDS.has(categoryId)) return TOURISM_ICON_SVG;
  if (WORSHIP_CATEGORY_IDS.has(categoryId)) return WORSHIP_ICON_SVG;
  if (HEALTH_CATEGORY_IDS.has(categoryId))  return HEALTH_ICON_SVG;
  return null;
}

export function buildPoiStyle(categoryId: number | null): Style {
  const iconSrc = getIconSrc(categoryId);

  if (iconSrc) {
    return new Style({
      image: new Icon({ src: iconSrc, scale: 0.6, anchor: [0.5, 0.5] }),
    });
  }

  return new Style({
    image: new CircleStyle({
      radius: 7,
      fill:   new Fill({ color: "#6b7280" }),
      stroke: new Stroke({ color: "#374151", width: 2 }),
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
