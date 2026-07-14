import { useEffect } from "react";
import { Feature }   from "ol";
import Point         from "ol/geom/Point";
import VectorSource  from "ol/source/Vector";
import { WKT }       from "ol/format";
import { Style, Fill, Stroke, Circle as CircleStyle } from "ol/style";
import type { PoiResponseDto } from "../../../shared/types/poi";

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

const wktFormat = new WKT();

export const poiMarkerStyle = new Style({
  image: new CircleStyle({
    radius: 7,
    fill:   new Fill({ color: "#0d9488" }),
    stroke: new Stroke({ color: "#134e4a", width: 2 }),
  }),
});

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
  feature.setStyle(poiMarkerStyle);
  return feature;
}

export function usePoiLoader(map: unknown, source: VectorSource, apiFetch: ApiFetch) {
  useEffect(() => {
    if (!map) return;

    async function load() {
      try {
        const res = await apiFetch("/api/poi");
        if (!res.ok) return;   // poi_read yoksa sessizce boş kalır
        const data: PoiResponseDto[] = await res.json();
        for (const dto of data) source.addFeature(poiToFeature(dto));
      } catch { }
    }

    load();
  }, [map]);
}