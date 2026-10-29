import { useState } from "react";
import type OlMap from "ol/Map";
import { importOsmPois } from "../api/osmImportService";
import { extentToBbox, formatOsmImportResult, isBboxTooLarge } from "../osmImport";
import type { OsmImportResultDto } from "../types";
import type { ApiFetch } from "../../../../shared/api/apiFetch";
import type { ToastState } from "../../../../shared/components/Toast";

// Imports OSM POIs for the current view extent (admin only; the backend enforces admin_access).
export function useOsmImport(
  map:        OlMap | null,
  apiFetch:   ApiFetch,
  onToast:    (toast: ToastState) => void,
  onImported: () => void,
) {
  const [busy, setBusy] = useState(false);

  async function run() {
    if (!map || busy) return;

    const bbox = extentToBbox(map.getView().calculateExtent(map.getSize()));
    if (isBboxTooLarge(bbox)) {
      onToast({ message: "Görünen alan çok büyük. İçe aktarmak için haritayı yakınlaştırın.", type: "error" });
      return;
    }

    setBusy(true);
    try {
      const res = await importOsmPois(apiFetch, bbox);
      if (!res.ok) {
        const message = await res.text();
        onToast({ message: message || "OSM içe aktarma başarısız oldu.", type: "error" });
        return;
      }

      const result: OsmImportResultDto = await res.json();
      onToast({ message: formatOsmImportResult(result), type: "success" });
      onImported();
    } catch {
      onToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
    } finally {
      setBusy(false);
    }
  }

  return { busy, run };
}
