interface RouteInfoPopupProps {
  x: number;
  y: number;
  routeName:          string;
  canManage:           boolean;
  isSimulationRunning: boolean;
  isTrackingThisRoute: boolean;
  isBusy:              boolean;
  onClose:             () => void;
  onStart:             () => void;
  onStop:              () => void;
  onTrack:             () => void;
  onStopTracking:      () => void;
}

export function RouteInfoPopup({
  x, y, routeName, canManage, isSimulationRunning, isTrackingThisRoute, isBusy,
  onClose, onStart, onStop, onTrack, onStopTracking,
}: RouteInfoPopupProps) {
  return (
    <div style={{
      position: "absolute", left: x, top: y, zIndex: 1500,
      background: "#ffffff", borderRadius: 10, padding: "12px 14px",
      boxShadow: "0 8px 24px rgba(0,0,0,0.18)", minWidth: 200,
    }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 10 }}>
        <span style={{ fontSize: 13, fontWeight: 600, color: "#0f172a" }}>{routeName}</span>
        <button
          onClick={onClose}
          style={{ border: "none", background: "transparent", color: "#94a3b8", fontSize: 14, cursor: "pointer" }}
        >
          ✕
        </button>
      </div>

      {canManage && (
        <div style={{ marginBottom: 8 }}>
          {isSimulationRunning ? (
            <button
              onClick={onStop}
              disabled={isBusy}
              style={{
                width: "100%", padding: "7px 0", borderRadius: 6, border: "none",
                background: "#ef4444", color: "#ffffff", fontSize: 12, fontWeight: 600, cursor: "pointer",
              }}
            >
              Sevkiyatı Durdur
            </button>
          ) : (
            <button
              onClick={onStart}
              disabled={isBusy}
              style={{
                width: "100%", padding: "7px 0", borderRadius: 6, border: "none",
                background: "#0f172a", color: "#ffffff", fontSize: 12, fontWeight: 600, cursor: "pointer",
              }}
            >
              Sevkiyat Başlat
            </button>
          )}
        </div>
      )}

      {isSimulationRunning && (
        isTrackingThisRoute ? (
          <button
            onClick={onStopTracking}
            style={{
              width: "100%", padding: "7px 0", borderRadius: 6,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 12, fontWeight: 500, cursor: "pointer",
            }}
          >
            Takibi Bırak
          </button>
        ) : (
          <button
            onClick={onTrack}
            style={{
              width: "100%", padding: "7px 0", borderRadius: 6, border: "none",
              background: "#3b82f6", color: "#ffffff", fontSize: 12, fontWeight: 600, cursor: "pointer",
            }}
          >
            Takip Et
          </button>
        )
      )}

      {!isSimulationRunning && !canManage && (
        <p style={{ fontSize: 12, color: "#94a3b8", margin: 0 }}>Şu anda çalışan bir sevkiyat yok.</p>
      )}
    </div>
  );
}