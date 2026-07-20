import { useState } from "react";
import type { Feature } from "ol";
import { WKT } from "ol/format";
import { useAuth } from "../../../auth/context/AuthContext";
import { buildStyle } from "../../../../utils/mapStyle";
import type { UserLookupEntry } from "../../core/hooks/useUserLookup";
import { ConfirmModal } from "../../../../shared/components/ConfirmModal";
import { updateTransitStop, deleteTransitStop } from "../../../../shared/api/transitService";
import type { TransitRouteResponseDto } from "../types";

const wktFormat = new WKT();

interface StopInfoPopupProps {
  feature:    Feature;
  routes:     TransitRouteResponseDto[];
  userLookup: Map<number, UserLookupEntry>;
  canManage:  boolean;
  onClose:    () => void;
  onUpdated:  () => void;
  onDeleted:  () => void;
}

export function StopInfoPopup({ feature, routes, userLookup, canManage, onClose, onUpdated, onDeleted }: StopInfoPopupProps) {
  const { apiFetch } = useAuth();

  const [isEditing,   setIsEditing]   = useState(false);
  const [name,        setName]        = useState(feature.get("stopName") as string);
  const [routeId,     setRouteId]     = useState<number>(feature.get("stopRouteId") as number);
  const [showConfirm, setShowConfirm] = useState(false);
  const [isSaving,    setIsSaving]    = useState(false);
  const [isDeleting,  setIsDeleting]  = useState(false);
  const [error,       setError]       = useState<string | null>(null);

  const userId      = feature.get("stopUserId") as number;
  const createdDate = feature.get("stopCreatedDate") as string;
  const stopId      = feature.get("stopId") as number;

  const currentRoute = routes.find((r) => r.id === (feature.get("stopRouteId") as number));
  const routeLabel   = currentRoute?.name ?? "—";
  const creatorEmail = userLookup.get(userId)?.email ?? "Bilinmiyor";

  async function handleSave() {
    setIsSaving(true);
    setError(null);
    try {
      const geometry = feature.getGeometry();
      if (!geometry) return;
      const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");
      const wktGeometry = wktFormat.writeGeometry(cloned);

      const res = await updateTransitStop(apiFetch, stopId, { name, transitRouteId: routeId, wktGeometry });

      if (!res.ok) {
        setError(res.status === 403 ? "Bu durağı düzenleme yetkiniz yok." : "Güncelleme başarısız.");
        return;
      }

      const newRoute = routes.find((r) => r.id === routeId);
      feature.set("stopName", name);
      feature.set("stopRouteId", routeId);
      feature.set("stopRouteName", newRoute?.name ?? "");
      feature.setStyle(buildStyle(newRoute?.color ?? "#3b82f6", name));

      setIsEditing(false);
      onUpdated();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDelete() {
    setShowConfirm(false);
    setIsDeleting(true);
    setError(null);
    try {
      const res = await deleteTransitStop(apiFetch, stopId);
      if (!res.ok) {
        setError(res.status === 403 ? "Bu durağı silme yetkiniz yok." : "Silme başarısız.");
        setIsDeleting(false);
        return;
      }
      onDeleted();
    } catch {
      setError("Sunucuya bağlanılamadı.");
      setIsDeleting(false);
    }
  }

  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, zIndex: 2000,
        background: "rgba(0,0,0,0.4)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "28px 32px", width: 360,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
        }}
      >
        <span style={{ fontSize: 11, fontWeight: 600, color: "#0d9488", letterSpacing: ".8px", textTransform: "uppercase" }}>
          Durak
        </span>

        {!isEditing ? (
          <>
            <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "4px 0 20px" }}>
              {feature.get("stopName") as string}
            </h2>

            <InfoRow label="Güzergah"          value={routeLabel} />
            <InfoRow label="Ekleyen"           value={creatorEmail} />
            <InfoRow label="Oluşturma Tarihi"  value={new Date(createdDate).toLocaleString("tr-TR")} />

            {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "12px 0 0" }}>{error}</p>}

            <div style={{ display: "flex", gap: 8, marginTop: 16 }}>
              <button
                onClick={onClose}
                style={{
                  flex: 1, padding: "9px 0", borderRadius: 8,
                  border: "1px solid #e2e8f0", background: "#f8fafc",
                  color: "#64748b", fontSize: 13, cursor: "pointer",
                }}
              >
                Kapat
              </button>

              {canManage && (
                <>
                  <button
                    onClick={() => setIsEditing(true)}
                    style={{
                      flex: 1, padding: "9px 0", borderRadius: 8,
                      border: "1px solid rgba(59,130,246,.3)",
                      background: "rgba(59,130,246,.05)",
                      color: "#3b82f6", fontSize: 13, fontWeight: 500, cursor: "pointer",
                    }}
                  >
                    Düzenle
                  </button>
                  <button
                    onClick={() => setShowConfirm(true)}
                    disabled={isDeleting}
                    style={{
                      flex: 1, padding: "9px 0", borderRadius: 8,
                      border: "1px solid rgba(239,68,68,.3)",
                      background: "rgba(239,68,68,.05)",
                      color: "#ef4444", fontSize: 13, fontWeight: 500,
                      cursor: isDeleting ? "not-allowed" : "pointer",
                      opacity: isDeleting ? 0.6 : 1,
                    }}
                  >
                    {isDeleting ? "Siliniyor..." : "Sil"}
                  </button>
                </>
              )}
            </div>
          </>
        ) : (
          <>
            <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "4px 0 20px" }}>
              Durak Düzenle
            </h2>

            <div style={{ marginBottom: 14 }}>
              <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
                Durak İsmi
              </label>
              <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                style={{
                  width: "100%", boxSizing: "border-box", padding: "8px 12px",
                  borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
                  color: "#0f172a", outline: "none",
                }}
              />
            </div>

            <div style={{ marginBottom: 20 }}>
              <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
                Güzergah
              </label>
              <select
                value={routeId}
                onChange={(e) => setRouteId(Number(e.target.value))}
                style={{
                  width: "100%", boxSizing: "border-box", padding: "8px 12px",
                  borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
                  color: "#0f172a", outline: "none", background: "#ffffff",
                }}
              >
                {routes.map((r) => (
                  <option key={r.id} value={r.id}>{r.name}</option>
                ))}
              </select>
            </div>

            {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 16px" }}>{error}</p>}

            <div style={{ display: "flex", gap: 10 }}>
              <button
                onClick={() => { setIsEditing(false); setError(null); }}
                disabled={isSaving}
                style={{
                  flex: 1, padding: "9px 0", borderRadius: 8,
                  border: "1px solid #e2e8f0", background: "#f8fafc",
                  color: "#64748b", fontSize: 13, cursor: "pointer",
                }}
              >
                İptal
              </button>
              <button
                onClick={handleSave}
                disabled={isSaving || !name.trim()}
                style={{
                  flex: 2, padding: "9px 0", borderRadius: 8, border: "none",
                  background: isSaving ? "#94a3b8" : "#0f172a",
                  color: "#ffffff", fontSize: 13, fontWeight: 600,
                  cursor: isSaving ? "not-allowed" : "pointer",
                }}
              >
                {isSaving ? "Kaydediliyor..." : "Kaydet"}
              </button>
            </div>
          </>
        )}
      </div>

      {showConfirm && (
        <ConfirmModal
          message={`"${feature.get("stopName") as string}" durağını silmek istediğinize emin misiniz?`}
          onConfirm={handleDelete}
          onCancel={() => setShowConfirm(false)}
        />
      )}
    </div>
  );
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <div style={{ marginBottom: 12 }}>
      <span style={{ display: "block", fontSize: 11, fontWeight: 500, color: "#94a3b8", marginBottom: 2 }}>
        {label}
      </span>
      <span style={{ fontSize: 14, color: "#0f172a" }}>{value}</span>
    </div>
  );
}