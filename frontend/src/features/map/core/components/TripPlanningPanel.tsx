import { useState } from "react";
import { useAuth } from "../../../auth/context/AuthContext";
import { planTrip, type TripPlanResultDto } from "../../../../shared/api/tripPlanningService";

type PickMode = "start" | "end" | null;

interface TripPoint {
  lon: number;
  lat: number;
}

interface TripPlanningPanelProps {
  onClose:           () => void;
  pickMode:          PickMode;
  onPickModeChange:  (mode: PickMode) => void;
  startPoint:        TripPoint | null;
  endPoint:          TripPoint | null;
  onResult:          (result: TripPlanResultDto | null) => void;
}

function formatDuration(seconds: number): string {
  const minutes = Math.round(seconds / 60);
  if (minutes < 60) return `${minutes} dk`;
  return `${Math.floor(minutes / 60)} sa ${minutes % 60} dk`;
}

function formatDistance(meters: number): string {
  return meters >= 1000 ? `${(meters / 1000).toFixed(1)} km` : `${Math.round(meters)} m`;
}

export function TripPlanningPanel({
  onClose, pickMode, onPickModeChange, startPoint, endPoint, onResult,
}: TripPlanningPanelProps) {
  const { apiFetch } = useAuth();

  const [result,     setResult]     = useState<TripPlanResultDto | null>(null);
  const [isLoading,  setIsLoading]  = useState(false);
  const [error,      setError]      = useState<string | null>(null);

  async function handlePlan() {
    if (!startPoint || !endPoint) return;
    setIsLoading(true);
    setError(null);
    setResult(null);
    onResult(null);

    try {
      const res = await planTrip(apiFetch, startPoint.lon, startPoint.lat, endPoint.lon, endPoint.lat);
      if (!res.ok) {
        const message = res.status === 404 ? await res.text() : "Rota hesaplanamadı.";
        setError(message);
        return;
      }
      const data: TripPlanResultDto = await res.json();
      setResult(data);
      onResult(data);
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div style={{
      position: "fixed", top: 70, left: 16, bottom: 100, width: 320,
      background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0",
      boxShadow: "0 8px 32px rgba(0,0,0,0.15)", zIndex: 950,
      display: "flex", flexDirection: "column", overflow: "hidden",
    }}>
      <div style={{
        display: "flex", alignItems: "center", justifyContent: "space-between",
        padding: "12px 16px", borderBottom: "1px solid #f1f5f9",
      }}>
        <span style={{ fontSize: 14, fontWeight: 600, color: "#0f172a" }}>Toplu Taşıma Yol Tarifi</span>
        <button
          onClick={onClose}
          style={{ border: "none", background: "transparent", color: "#94a3b8", fontSize: 16, cursor: "pointer" }}
        >
          ✕
        </button>
      </div>

      <div style={{ padding: 16, borderBottom: "1px solid #f1f5f9", display: "flex", flexDirection: "column", gap: 8 }}>
        <button
          onClick={() => onPickModeChange(pickMode === "start" ? null : "start")}
          style={{
            display: "flex", alignItems: "center", gap: 8, padding: "8px 10px",
            borderRadius: 8, border: pickMode === "start" ? "2px solid #3b82f6" : "1px solid #e2e8f0",
            background: pickMode === "start" ? "rgba(59,130,246,.05)" : "#ffffff",
            fontSize: 13, color: "#0f172a", cursor: "pointer", textAlign: "left",
          }}
        >
          <span style={{ width: 10, height: 10, borderRadius: "50%", background: "#22c55e", flexShrink: 0 }} />
          {startPoint ? `Başlangıç: ${startPoint.lat.toFixed(5)}, ${startPoint.lon.toFixed(5)}` : "Başlangıç noktası seçin (haritaya tıklayın)"}
        </button>

        <button
          onClick={() => onPickModeChange(pickMode === "end" ? null : "end")}
          style={{
            display: "flex", alignItems: "center", gap: 8, padding: "8px 10px",
            borderRadius: 8, border: pickMode === "end" ? "2px solid #3b82f6" : "1px solid #e2e8f0",
            background: pickMode === "end" ? "rgba(59,130,246,.05)" : "#ffffff",
            fontSize: 13, color: "#0f172a", cursor: "pointer", textAlign: "left",
          }}
        >
          <span style={{ width: 10, height: 10, borderRadius: "50%", background: "#ef4444", flexShrink: 0 }} />
          {endPoint ? `Bitiş: ${endPoint.lat.toFixed(5)}, ${endPoint.lon.toFixed(5)}` : "Bitiş noktası seçin (haritaya tıklayın)"}
        </button>

        <button
          onClick={handlePlan}
          disabled={!startPoint || !endPoint || isLoading}
          style={{
            padding: "9px 0", borderRadius: 8, border: "none",
            background: !startPoint || !endPoint || isLoading ? "#94a3b8" : "#0f172a",
            color: "#ffffff", fontSize: 13, fontWeight: 600,
            cursor: !startPoint || !endPoint || isLoading ? "not-allowed" : "pointer",
          }}
        >
          {isLoading ? "Hesaplanıyor..." : "Yol Tarifi Bul"}
        </button>
      </div>

      <div style={{ flex: 1, overflowY: "auto", padding: 16 }}>
        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 12px" }}>{error}</p>}

        {result && (
          <>
            <div style={{ marginBottom: 14 }}>
              <p style={{ fontSize: 20, fontWeight: 700, color: "#0f172a", margin: 0 }}>
                {formatDuration(result.totalDurationSeconds)}
              </p>
              <p style={{ fontSize: 12, color: "#94a3b8", margin: 0 }}>
                Toplam yürüme: {formatDistance(result.totalWalkingMeters)}
              </p>
            </div>

            <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
              {result.legs.map((leg, i) => (
                <div
                  key={i}
                  style={{
                    display: "flex", gap: 10, padding: "8px 10px",
                    borderRadius: 8, border: "1px solid #e2e8f0",
                  }}
                >
                  <div style={{
                    width: 28, height: 28, borderRadius: "50%", flexShrink: 0,
                    display: "flex", alignItems: "center", justifyContent: "center",
                    background: leg.type === "Bus" ? (leg.routeColor ?? "#3b82f6") : "#f1f5f9",
                    color: leg.type === "Bus" ? "#ffffff" : "#64748b",
                    fontSize: 13,
                  }}>
                    {leg.type === "Bus" ? "🚌" : "🚶"}
                  </div>
                  <div style={{ flex: 1 }}>
                    <p style={{ fontSize: 13, fontWeight: 500, color: "#0f172a", margin: 0 }}>
                      {leg.type === "Bus"
                        ? `Otobüse Bin: ${leg.routeName}`
                        : "Yürü"}
                    </p>
                    <p style={{ fontSize: 11, color: "#94a3b8", margin: "2px 0 0" }}>
                      {leg.fromName} → {leg.toName}
                    </p>
                    <p style={{ fontSize: 11, color: "#94a3b8", margin: "2px 0 0" }}>
                      {formatDuration(leg.durationSeconds)} · {formatDistance(leg.distanceMeters)}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </>
        )}

        {!result && !error && (
          <p style={{ fontSize: 13, color: "#94a3b8", textAlign: "center", padding: "20px 0" }}>
            Başlangıç ve bitiş noktalarını seçip yol tarifi hesaplayın.
          </p>
        )}
      </div>
    </div>
  );
}