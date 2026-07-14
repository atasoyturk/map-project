import { useEffect, useState } from "react";
import OlMap  from "ol/Map";
import Point  from "ol/geom/Point";
import { WKT } from "ol/format";
import type { PendingAnnotation } from "../types";

interface UseAnnotationContextMenuOptions {
  map:     OlMap | null;
  enabled: boolean;
}

const wktFormat = new WKT();

export function useAnnotationContextMenu({ map, enabled }: UseAnnotationContextMenuOptions) {
  const [pending, setPending] = useState<PendingAnnotation | null>(null);

  useEffect(() => {
    if (!map) return;
    if (!enabled) { setPending(null); return; }

    const viewport = map.getViewport();

    function handleContextMenu(evt: MouseEvent) {
      evt.preventDefault();

      const pixel = map!.getEventPixel(evt);
      const coordinate = map!.getCoordinateFromPixel(pixel);
      if (!coordinate) return;

      const geometry3857 = new Point(coordinate);
      const geometry4326  = geometry3857.clone().transform("EPSG:3857", "EPSG:4326");
      const wkt = wktFormat.writeGeometry(geometry4326);

      setPending({ wkt, x: pixel[0], y: pixel[1] });
    }

    viewport.addEventListener("contextmenu", handleContextMenu);
    return () => {
      viewport.removeEventListener("contextmenu", handleContextMenu);
      setPending(null);
    };
  }, [map, enabled]);

  return { pending, clear: () => setPending(null) };
}