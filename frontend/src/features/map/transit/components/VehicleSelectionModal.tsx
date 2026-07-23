import { useState, useEffect } from "react";
import { useAuth } from "../../../auth/context/AuthContext";
import { getCompaniesByRoute, getVehiclesByCompany } from "../../../../shared/api/companyService";
import type { VehicleResponseDto } from "../../../company/types";

interface VehicleSelectionModalProps {
  routeId:            number;
  mode:                "start" | "stop";
  runningVehicleIds:   Set<number>;
  onClose:             () => void;
  onConfirm:           (vehicleIds: number[]) => Promise<void>;
}

export function VehicleSelectionModal({
  routeId, mode, runningVehicleIds, onClose, onConfirm,
}: VehicleSelectionModalProps) {
  const { apiFetch } = useAuth();

  const [vehicles,   setVehicles]   = useState<VehicleResponseDto[]>([]);
  const [selected,   setSelected]   = useState<Set<number>>(new Set());
  const [isLoading,  setIsLoading]  = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error,      setError]      = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      setIsLoading(true);
      try {
        const companiesRes = await getCompaniesByRoute(apiFetch, routeId);
        if (!companiesRes.ok) { setError("Şirketler yüklenemedi."); return; }
        const companies: { id: number }[] = await companiesRes.json();

        const vehicleLists = await Promise.all(
          companies.map(async (c) => {
            const res = await getVehiclesByCompany(apiFetch, c.id);
            return res.ok ? await res.json() as VehicleResponseDto[] : [];
          }),
        );

        setVehicles(vehicleLists.flat());
      } catch {
        setError("Sunucuya bağlanılamadı.");
      } finally {
        setIsLoading(false);
      }
    })();
  }, [routeId]);

  const relevantVehicles = mode === "start"
    ? vehicles.filter((v) => !runningVehicleIds.has(v.id))
    : vehicles.filter((v) => runningVehicleIds.has(v.id));

  function toggle(vehicleId: number) {
    setSelected((prev) => {
      const next = new Set(prev);
      next.has(vehicleId) ? next.delete(vehicleId) : next.add(vehicleId);
      return next;
    });
  }

  async function handleConfirm() {
    if (selected.size === 0) return;
    setIsSubmitting(true);
    try {
      await onConfirm(Array.from(selected));
    } finally {
      setIsSubmitting(false);
    }
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
          padding: "28px 32px", width: 380, maxHeight: "70vh",
          display: "flex", flexDirection: "column",
          boxShadow: "0 20px 60px rgba(0,0,0,0.3)",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 4px" }}>
          {mode === "start" ? "Sevkiyat Başlat" : "Sevkiyatı Durdur"}
        </h2>
        <p style={{ fontSize: 13, color: "#64748b", margin: "0 0 16px" }}>
          {mode === "start"
            ? "Sevkiyata başlayacak araçları seçin."
            : "Durdurulacak araçları seçin."}
        </p>

        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 12px" }}>{error}</p>}

        {isLoading ? (
          <p style={{ fontSize: 13, color: "#94a3b8" }}>Yükleniyor...</p>
        ) : relevantVehicles.length === 0 ? (
          <p style={{ fontSize: 13, color: "#94a3b8" }}>
            {mode === "start"
              ? "Bu güzergaha atanmış müsait araç yok."
              : "Bu güzergahta çalışan araç yok."}
          </p>
        ) : (
          <div style={{ overflowY: "auto", display: "flex", flexDirection: "column", gap: 6, marginBottom: 20 }}>
            {relevantVehicles.map((vehicle) => (
              <label
                key={vehicle.id}
                style={{
                  display: "flex", alignItems: "center", gap: 8,
                  padding: "8px 10px", borderRadius: 8,
                  border: "1px solid #e2e8f0", fontSize: 13, color: "#374151", cursor: "pointer",
                }}
              >
                <input
                  type="checkbox"
                  checked={selected.has(vehicle.id)}
                  onChange={() => toggle(vehicle.id)}
                />
                <span style={{ flex: 1 }}>{vehicle.plateNumber}</span>
                <span style={{ fontSize: 11, color: "#94a3b8" }}>{vehicle.companyName}</span>
              </label>
            ))}
          </div>
        )}

        <div style={{ display: "flex", gap: 8 }}>
          <button
            onClick={onClose}
            style={{
              flex: 1, padding: "9px 0", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={handleConfirm}
            disabled={selected.size === 0 || isSubmitting}
            style={{
              flex: 1, padding: "9px 0", borderRadius: 8, border: "none",
              background: mode === "start" ? "#0f172a" : "#ef4444",
              color: "#ffffff", fontSize: 13, fontWeight: 600,
              cursor: selected.size === 0 || isSubmitting ? "not-allowed" : "pointer",
              opacity: selected.size === 0 || isSubmitting ? 0.6 : 1,
            }}
          >
            {isSubmitting ? "İşleniyor..." : mode === "start" ? "Başlat" : "Durdur"}
          </button>
        </div>
      </div>
    </div>
  );
}