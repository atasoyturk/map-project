import { useEffect } from "react";
import type OlMap from "ol/Map";
import TileLayer   from "ol/layer/Tile";
import TileWMS     from "ol/source/TileWMS";
import type { DrawingLayers } from "./useDrawing";

export function useWmsHeatmapLayer(
  map: OlMap | null,
  active: boolean,
  token: string | null,
  layers: DrawingLayers | null,
  enabled: boolean,
) {
  useEffect(() => {
    if (!map || !enabled) return;

    if (layers?.pointLayer) {
      layers.pointLayer.setVisible(!active);
    }

    if (!active) return;

    const heatmapLayer = new TileLayer({
      source: new TileWMS({
        url:    "http://localhost:5130/api/proxy/geoserver/wms",
        params: {
          typeName:    "tbl_point",
          styles:      "gisportal:gisportal_heatmap",
          FORMAT:      "image/png",
          TRANSPARENT: true,
        },
        tileLoadFunction: (tile: any, src: string) => {
          fetch(src, {
            headers: { Authorization: `Bearer ${token}` },
          })
            .then((res) => res.blob())
            .then((blob) => {
              tile.getImage().src = URL.createObjectURL(blob);
            });
        },
      }),
      zIndex:   10,
      opacity:  0.75,
    });

    map.addLayer(heatmapLayer);

    return () => {
      map.removeLayer(heatmapLayer);
      if (layers?.pointLayer) {
        layers.pointLayer.setVisible(true);
      }
    };
  }, [map, active, token, layers, enabled]);
}
