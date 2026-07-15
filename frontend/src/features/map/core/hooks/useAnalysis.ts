import { useEffect, useRef } from "react";
import Map from "ol/Map";
import Draw from "ol/interaction/Draw";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { WKT } from "ol/format";
import { Style, Fill, Stroke, Circle} from "ol/style";
import { validateGeometry } from "../api/geoService";
import { analyzeTempInventory } from "../api/analysisService";

interface UseAnalysisOptions {
  map:       Map | null;
  active:    boolean;
  apiFetch:  (path: string, options?: RequestInit) => Promise<Response>;
  onResult:  (count: number) => void;
  onError:   (msg: string) => void;
}

export function useAnalysis({
  map,
  active,
  apiFetch,
  onResult,
  onError,
}: UseAnalysisOptions) {
  const drawRef   = useRef<Draw | null>(null);
  const sourceRef = useRef(new VectorSource());
  const layerRef  = useRef<VectorLayer<VectorSource> | null>(null);

  useEffect(() => {
    if (!map) return;

    const layer = new VectorLayer({
      source: sourceRef.current,
      zIndex: 2,
      style: new Style({
        fill:   new Fill({ color: "rgba(234,179,8,0.15)" }),
        stroke: new Stroke({ color: "#eab308", width: 2, lineDash: [6, 4] }),
      }),
    });

    map.addLayer(layer);
    layerRef.current = layer;

    return () => {
      map.removeLayer(layer);
      layerRef.current = null;
    };
  }, [map]);

  useEffect(() => {
    if (!map) return;

    if (drawRef.current) {
      map.removeInteraction(drawRef.current);
      drawRef.current = null;
    }

    if (!active) return;

    const draw = new Draw({
      source: sourceRef.current,
      type:   "Polygon",
      style:  new Style({
        fill:   new Fill({ color: "rgba(234,179,8,0.15)" }),
        stroke: new Stroke({ color: "#eab308", width: 2, lineDash: [6, 4] }),
        image:  new Circle({
          radius: 6,
          fill:   new Fill({ color: "#eab308" }),
          stroke: new Stroke({ color: "#ffffff", width: 2 }),
        }),
      }),
    });

    draw.on("drawend", async (event) => {
      const geometry = event.feature.getGeometry();
      if (!geometry) return;

      const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");
      const wkt    = new WKT().writeGeometry(cloned);

      try {
        const validateRes = await validateGeometry(apiFetch, wkt);

        if (!validateRes.ok) {
          sourceRef.current.clear();
          const message = await validateRes.text();
          onError(`Hata: ${message}`);
          return;
        }

        const response = await analyzeTempInventory(apiFetch, wkt);

        if (!response.ok) { onError("Analiz başarısız."); return; }

        const data = await response.json();
        onResult(data.intersectedInventoryCount);
      } catch {
        onError("Sunucuya bağlanılamadı.");
      }
    });

    map.addInteraction(draw);
    drawRef.current = draw;

    return () => {
      map.removeInteraction(draw);
      drawRef.current = null;
    };
  }, [map, active]);

  function clear() {
    sourceRef.current.clear();
  }

  return { clear };
}