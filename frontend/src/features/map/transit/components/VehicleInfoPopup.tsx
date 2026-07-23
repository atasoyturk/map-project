interface VehicleInfoPopupProps {
  x: number;
  y: number;
  routeName:           string;
  plateNumber:         string;
  progressPercentage:  number;
  onClose:             () => void;
  onStopTracking:      () => void;
}

export function VehicleInfoPopup({
  x, y, routeName, plateNumber, progressPercentage, onClose, onStopTracking,
}: VehicleInfoPopupProps) {
  return (
    <div style={{
      position: "absolute", left: x, top: y, zIndex: 1500,
      background: "#ffffff", borderRadius: 10, padding: "12px 14px",
      boxShadow: "0 8px 24px rgba(0,0,0,0.18)", minWidth: 200,
    }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 8 }}>
        <div>
          <div style={{ fontSize: 13, fontWeight: 600, color: "#0f172a" }}>{plateNumber}</div>
          <div style={{ fontSize: 11, color: "#94a3b8" }}>{routeName}</div>
        </div>
        <button
          onClick={onClose}
          style={{ border: "none", background: "transparent", color: "#94a3b8", fontSize: 14, cursor: "pointer" }}
        >
          ✕
        </button>
      </div>

      <p style={{ fontSize: 12, color: "#64748b", margin: "0 0 10px" }}>
        İlerleme: <strong style={{ color: "#0f172a" }}>%{progressPercentage.toFixed(1)}</strong>
      </p>

      <button
        onClick={onStopTracking}
        style={{
          width: "100%", padding: "6px 0", borderRadius: 6,
          border: "1px solid #e2e8f0", background: "#f8fafc",
          color: "#64748b", fontSize: 12, fontWeight: 500, cursor: "pointer",
        }}
      >
        Takibi Bırak
      </button>
    </div>
  );
}