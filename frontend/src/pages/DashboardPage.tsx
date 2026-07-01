import { useState }     from "react";
import Map              from "ol/Map";
import { MapView }      from "../components/MapView";
import { Navbar }       from "../components/Navbar";

export function DashboardPage() {
  const [map, setMap] = useState<Map | null>(null);

  return (
    <div style={{ display: "flex", flexDirection: "column", height: "100vh" }}>
      <Navbar map={map} />
      <div style={{ marginTop: 56, flex: 1 }}>
        <MapView onMapReady={setMap} height="calc(100vh - 56px)" />
      </div>
    </div>
  );
}