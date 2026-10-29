import { transformExtent } from "ol/proj";
import type { Extent } from "ol/extent";
import type { OsmImportBbox, OsmImportResultDto } from "./types";

// Same limit as the backend (OsmImportService.MaxBboxSizeDegrees).
export const MAX_OSM_BBOX_DEGREES = 0.5;

export function extentToBbox(extent: Extent): OsmImportBbox {
  const [minLon, minLat, maxLon, maxLat] = transformExtent(extent, "EPSG:3857", "EPSG:4326");
  return { minLon, minLat, maxLon, maxLat };
}

export function isBboxTooLarge(bbox: OsmImportBbox): boolean {
  return bbox.maxLon - bbox.minLon > MAX_OSM_BBOX_DEGREES || bbox.maxLat - bbox.minLat > MAX_OSM_BBOX_DEGREES;
}

export function formatOsmImportResult(result: OsmImportResultDto): string {
  const counts = `${result.added} eklendi, ${result.updated} güncellendi, ${result.skipped} atlandı`;
  const missing = result.skippedCategories.length > 0
    ? ` (${result.skippedCategories.length} kategori artık mevcut olmadığı için atlandı)`
    : "";
  return `OSM içe aktarma tamamlandı: ${counts}${missing}.`;
}
