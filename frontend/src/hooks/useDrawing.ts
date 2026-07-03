import { useEffect, useRef } from "react";
import Map from "ol/Map";
import Draw from "ol/interaction/Draw";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { WKT } from "ol/format";
import type { DrawType, PendingGeometry } from "../types/drawing";

interface UseDrawingOptions {
  map:        Map | null;
  source:     VectorSource;
  activeType: DrawType | null;
  onDrawEnd:  (pending: PendingGeometry) => void;
}

export function useDrawing({
  map,
  source,
  activeType,
  onDrawEnd,
}: UseDrawingOptions) {
  const drawRef = useRef<Draw | null>(null);

  // VectorLayer
  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({ source, zIndex: 1 });
    map.addLayer(layer);
    return () => { map.removeLayer(layer); };
  }, [map]);

  // Draw interaction
  useEffect(() => {
    if (!map) return;

    if (drawRef.current) {
      map.removeInteraction(drawRef.current);
      drawRef.current = null;
    }

    if (!activeType) return;

    const draw = new Draw({ source, type: activeType });

    draw.on("drawend", (event) => {
      const geometry = event.feature.getGeometry();
      if (!geometry) return;

      const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");
      const wkt    = new WKT().writeGeometry(cloned);

      onDrawEnd({ wkt, type: activeType, feature: event.feature });
    });

    map.addInteraction(draw);
    drawRef.current = draw;

    return () => {
      map.removeInteraction(draw);
      drawRef.current = null;
    };
  }, [map, activeType]);
}