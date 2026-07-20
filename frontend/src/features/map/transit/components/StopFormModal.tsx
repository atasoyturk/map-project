import { useState } from "react";
import type { TransitRouteResponseDto } from "../types";

interface StopFormModalProps {
  routes:        TransitRouteResponseDto[];
  lockedRouteId?: number | null;
  onSave:        (data: { name: string; transitRouteId: number }) => void;
  onCancel:      () => void;
  isSaving:      boolean;
  error?:        string | null;
}

export function StopFormModal({ routes, lockedRouteId, onSave, onCancel, isSaving, error }: StopFormModalProps) {
  const [name,    setName]    = useState("");
  const [routeId, setRouteId] = useState<number | "">(lockedRouteId ?? "");

  const isLocked = lockedRouteId != null;
  const canSave  = !!name.trim() && routeId !== "" && !isSaving;

  function handleSave() {
    if (!canSave) return;
    onSave({ name: name.trim(), transitRouteId: routeId as number });
  }

  return (
    <div
      onClick={onCancel}
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
          padding: "28px 32px", width: 380,
          boxShadow: "0 20px 60px rgba(0,0,0,0.3)",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 20px" }}>
          Yeni Durak Ekle
        </h2>

        <div style={{ marginBottom: 16 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Durak İsmi
          </label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Örn: Merkez Durağı"
            style={{
              width: "100%", boxSizing: "border-box", padding: "8px 12px",
              borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
              color: "#0f172a", outline: "none",
            }}
          />
        </div>

        <div style={{ marginBottom: 24 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Güzergah
          </label>
          <select
            value={routeId}
            disabled={isLocked}
            onChange={(e) => setRouteId(e.target.value === "" ? "" : Number(e.target.value))}
            style={{
              width: "100%", boxSizing: "border-box", padding: "8px 12px",
              borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
              color: "#0f172a", outline: "none",
              background: isLocked ? "#f8fafc" : "#ffffff",
              cursor: isLocked ? "not-allowed" : "pointer",
            }}
          >
            <option value="">— Güzergah seçin —</option>
            {routes.map((r) => (
              <option key={r.id} value={r.id}>{r.name}</option>
            ))}
          </select>
        </div>

        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 16px" }}>{error}</p>}

        <div style={{ display: "flex", gap: 10 }}>
          <button
            onClick={onCancel}
            disabled={isSaving}
            style={{
              flex: 1, padding: "9px 0", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={handleSave}
            disabled={!canSave}
            style={{
              flex: 2, padding: "9px 0", borderRadius: 8, border: "none",
              background: !canSave ? "#94a3b8" : "#0f172a",
              color: "#ffffff", fontSize: 13, fontWeight: 600,
              cursor: !canSave ? "not-allowed" : "pointer",
            }}
          >
            {isSaving ? "Kaydediliyor..." : "Kaydet"}
          </button>
        </div>
      </div>
    </div>
  );
}