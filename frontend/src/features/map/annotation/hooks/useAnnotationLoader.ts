import { useEffect } from "react";
import { Feature }   from "ol";
import Point         from "ol/geom/Point";
import VectorSource  from "ol/source/Vector";
import { WKT }       from "ol/format";
import type { AnnotationResponseDto } from "../types";
import { getAllAnnotations } from "../api/annotationService";

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

const wktFormat = new WKT();

export function annotationToFeature(dto: any): Feature { 
  const geometry = wktFormat.readGeometry(dto.wktGeometry || dto.WktGeometry, {
    dataProjection:    "EPSG:4326",
    featureProjection: "EPSG:3857",
  }) as Point;

  const feature = new Feature({ geometry });
  
  // Hem PascalCase hem camelCase kontrolü
  const id = dto.id ?? dto.Id;
  const noteText = dto.noteText ?? dto.NoteText;
  const userId = dto.userId ?? dto.UserId;
  const teamId = dto.teamId ?? dto.TeamId;
  const createdDate = dto.createdDate ?? dto.CreatedDate;

  feature.set("id",          id);
  feature.set("noteText",    noteText);
  feature.set("userId",      userId);
  feature.set("teamId",      teamId);
  feature.set("createdDate", createdDate);
  feature.set("isAnnotation", true);
  
  if (id) feature.setId(id); 
  
  return feature;
}

export function useAnnotationLoader(map: unknown, source: VectorSource, apiFetch: ApiFetch, enabled: boolean = true) {
  useEffect(() => {
    if (!map || !enabled) return;

    async function load() {
      try {
        const res = await getAllAnnotations(apiFetch);
        if (!res.ok) return;

        const data: AnnotationResponseDto[] = await res.json();
        for (const dto of data) {
          source.addFeature(annotationToFeature(dto));
        }
      } catch {  }
    }

    load();
  }, [map, enabled]);
}