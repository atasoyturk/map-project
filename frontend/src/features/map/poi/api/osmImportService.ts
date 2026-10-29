import type { ApiFetch } from "../../../../shared/api/apiFetch";
import type { OsmImportBbox } from "../types";

export function importOsmPois(apiFetch: ApiFetch, bbox: OsmImportBbox) {
  return apiFetch("/api/poi/import-osm", {
    method: "POST",
    body:   JSON.stringify(bbox),
  });
}
