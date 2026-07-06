import { useEffect, useRef } from "react";
import Map from "ol/Map";
import Draw from "ol/interaction/Draw";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { WKT } from "ol/format";
import type { DrawType, PendingGeometry } from "../types/drawing";

interface UseDrawingOptions {
  map:          Map | null;
  pointSource:  VectorSource;
  lineSource:   VectorSource;
  polygonSource:VectorSource;
  activeType:   DrawType | null;
  onDrawEnd:    (pending: PendingGeometry) => void;
}

export interface DrawingLayers {
  pointLayer:   VectorLayer<VectorSource>;
  lineLayer:    VectorLayer<VectorSource>;
  polygonLayer: VectorLayer<VectorSource>;
}

export function useDrawing({
  map,
  pointSource,
  lineSource,
  polygonSource,
  activeType,
  onDrawEnd,
}: UseDrawingOptions): { layers: DrawingLayers | null } {

  const drawRef   = useRef<Draw | null>(null);
  const layersRef = useRef<DrawingLayers | null>(null);

  // 3 different VectorLayer
  useEffect(() => {
    if (!map) return;

    const pointLayer   = new VectorLayer({ source: pointSource,   zIndex: 1 });
    const lineLayer    = new VectorLayer({ source: lineSource,    zIndex: 1 });
    const polygonLayer = new VectorLayer({ source: polygonSource, zIndex: 1 });

    map.addLayer(pointLayer);
    map.addLayer(lineLayer);
    map.addLayer(polygonLayer);

    layersRef.current = { pointLayer, lineLayer, polygonLayer };

    return () => {
      map.removeLayer(pointLayer);
      map.removeLayer(lineLayer);
      map.removeLayer(polygonLayer);
      layersRef.current = null;
    };
  }, [map]);

  // Draw interaction
  useEffect(() => {
    if (!map) return;

    if (drawRef.current) {
      map.removeInteraction(drawRef.current);
      drawRef.current = null;
    }

    if (!activeType) return;

    const activeSource =
      activeType === "Point"      ? pointSource  :
      activeType === "LineString" ? lineSource   :
                                    polygonSource;

    const draw = new Draw({ source: activeSource, type: activeType });

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

  return { layers: layersRef.current };
}