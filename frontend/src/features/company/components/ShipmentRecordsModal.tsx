import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { getShipmentRecords } from "../../../shared/api/companyService";
import type { ShipmentRecordDto } from "../types";

interface ShipmentRecordsModalProps {
  transitRouteId?: number;
  title?:          string;
  onClose:         () => void;
}

export function ShipmentRecordsModal({ transitRouteId, title, onClose }: ShipmentRecordsModalProps) {
  const { apiFetch } = useAuth();

  const [records,   setRecords]   = useState<ShipmentRecordDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error,     setError]     = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      setIsLoading(true);
      try {
        const res = await getShipmentRecords(apiFetch, transitRouteId);
        if (!res.ok) { setError("Sevkiyat geçmişi yüklenemedi."); return; }
        setRecords(await res.json());
      } catch {
        setError("Sunucuya bağlanılamadı.");
      } finally {
        setIsLoading(false);
      }
    })();
  }, [transitRouteId]);

  function formatDuration(startedAtUtc: string, completedAtUtc: string): string {
    const seconds = Math.round(
      (new Date(completedAtUtc).getTime() - new Date(startedAtUtc).getTime()) / 1000);
    return `${seconds} sn`;
  }

  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, zIndex: 2000,
        background: "rgba(0,0,0,0.5)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "28px 32px", width: 520, maxHeight: "70vh",
          display: "flex", flexDirection: "column",
          boxShadow: "0 20px 60px rgba(0,0,0,0.3)",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 16px" }}>
          {title ?? "Tamamlanan Sevkiyatlar"}
        </h2>

        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 12px" }}>{error}</p>}

        {isLoading ? (
          <p style={{ fontSize: 13, color: "#94a3b8" }}>Yükleniyor...</p>
        ) : records.length === 0 ? (
          <p style={{ fontSize: 13, color: "#94a3b8" }}>Henüz tamamlanan sevkiyat yok.</p>
        ) : (
          <div style={{ overflowY: "auto", marginBottom: 20 }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12 }}>
              <thead>
                <tr style={{ background: "#f8fafc" }}>
                  <th style={thStyle}>Güzergah</th>
                  <th style={thStyle}>Plaka</th>
                  <th style={thStyle}>Şirket</th>
                  <th style={thStyle}>Süre</th>
                  <th style={thStyle}>Tamamlanma</th>
                </tr>
              </thead>
              <tbody>
                {records.map((r) => (
                  <tr key={r.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                    <td style={tdStyle}>{r.routeName}</td>
                    <td style={tdStyle}>{r.plateNumber}</td>
                    <td style={tdStyle}>{r.companyName}</td>
                    <td style={tdStyle}>{formatDuration(r.startedAtUtc, r.completedAtUtc)}</td>
                    <td style={tdStyle}>{new Date(r.completedAtUtc).toLocaleString("tr-TR")}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        <button
          onClick={onClose}
          style={{
            padding: "9px 0", borderRadius: 8,
            border: "1px solid #e2e8f0", background: "#f8fafc",
            color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
          }}
        >
          Kapat
        </button>
      </div>
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding: "8px 10px", textAlign: "left",
  fontSize: 10, fontWeight: 600, color: "#64748b", letterSpacing: ".5px",
};

const tdStyle: React.CSSProperties = {
  padding: "8px 10px", color: "#374151",
};