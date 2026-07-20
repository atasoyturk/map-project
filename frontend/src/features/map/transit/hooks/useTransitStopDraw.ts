import { useEffect, useRef } from "react";
import OlMap         from "ol/Map";
import Draw           from "ol/interaction/Draw";
import VectorSource   from "ol/source/Vector";
import { WKT }         from "ol/format";
import type { Feature } from "ol";

interface UseTransitStopDrawOptions {
  map:       OlMap | null;
  source:    VectorSource;
  active:    boolean;
  onDrawEnd: (wkt: string, feature: Feature) => void;
}

const wktFormat = new WKT();

export function useTransitStopDraw({ map, source, active, onDrawEnd }: UseTransitStopDrawOptions) {
  const drawRef = useRef<Draw | null>(null);

  useEffect(() => {
    if (!map) return;

    if (drawRef.current) {
      map.removeInteraction(drawRef.current);
      drawRef.current = null;
    }

    if (!active) return;

    const draw = new Draw({ source, type: "Point" });

    draw.on("drawend", (event) => {
      const geometry = event.feature.getGeometry();
      if (!geometry) return;

      const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");
      const wkt = wktFormat.writeGeometry(cloned);

      onDrawEnd(wkt, event.feature as Feature);
    });

    map.addInteraction(draw);
    drawRef.current = draw;

    return () => {
      map.removeInteraction(draw);
      drawRef.current = null;
    };
  }, [map, active]);
}