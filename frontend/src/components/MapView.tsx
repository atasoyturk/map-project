import { useEffect, useRef } from "react";
import Map from "ol/Map";
import View from "ol/View";
import TileLayer from "ol/layer/Tile";
import OSM from "ol/source/OSM";
import { fromLonLat } from "ol/proj";

const TURKEY_CENTER = fromLonLat([35.2433, 38.9637]);
const INITIAL_ZOOM  = 6;

interface MapViewProps {
  onMapReady?: (map: Map) => void;   
}

export function MapView({ onMapReady }: MapViewProps) {
  const mapRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!mapRef.current) return;

    const map = new Map({
      target: mapRef.current,
      layers: [new TileLayer({ source: new OSM() })],
      view: new View({ center: TURKEY_CENTER, zoom: INITIAL_ZOOM }),
    });

    onMapReady?.(map);   

    return () => { map.setTarget(undefined); };
  }, []);

  return <div ref={mapRef} style={{ width: "100%", height: "100vh", zIndex: 0 }} />;
}