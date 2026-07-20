import type { ApiFetch } from "../../core/api/geoService";

interface AnnotationPayload {
  noteText:    string;
  wktGeometry: string;
}

export function getAllAnnotations(apiFetch: ApiFetch) {
  return apiFetch("/api/annotation");
}

export function createAnnotation(apiFetch: ApiFetch, data: AnnotationPayload) {
  return apiFetch("/api/annotation", {
    method: "POST",
    body:   JSON.stringify(data),
  });
}

export function deleteAnnotation(apiFetch: any, id: number) {
  return apiFetch(`/api/annotation/${id}`, {
    method: "DELETE",
  });
}
