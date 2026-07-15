import { useEffect } from "react";
import { Feature }   from "ol";
import Point         from "ol/geom/Point";
import VectorSource  from "ol/source/Vector";
import { WKT }       from "ol/format";
import type { AnnotationResponseDto } from "../types";
import { getAllAnnotations } from "../api/annotationService";

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

const wktFormat = new WKT();

export function annotationToFeature(dto: AnnotationResponseDto): Feature {
  const geometry = wktFormat.readGeometry(dto.wktGeometry, {
    dataProjection:    "EPSG:4326",
    featureProjection: "EPSG:3857",
  }) as Point;

  const feature = new Feature({ geometry });
  feature.set("id",          dto.id);
  feature.set("noteText",    dto.noteText);
  feature.set("userId",      dto.userId);
  feature.set("teamId",      dto.teamId);
  feature.set("createdDate", dto.createdDate);
  feature.set("isAnnotation", true);
  return feature;
}

export function useAnnotationLoader(map: unknown, source: VectorSource, apiFetch: ApiFetch) {
  useEffect(() => {
    if (!map) return;

    async function load() {
      try {
        const res = await getAllAnnotations(apiFetch);
        if (!res.ok) return;

        const data: AnnotationResponseDto[] = await res.json();
        for (const dto of data) {
          source.addFeature(annotationToFeature(dto));
        }
      } catch { }
    }

    load();
  }, [map]);
}