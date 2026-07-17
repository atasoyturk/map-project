import { useState } from "react";
import type { Feature } from "ol";
import { WKT } from "ol/format";
import { useAuth } from "../../../auth/context/AuthContext";
import { flattenCategories, findCategoryPath, type CategoryTreeNode } from "../../../../shared/utils/categoryTree";
import type { UserLookupEntry } from "../../core/hooks/useUserLookup";
import { ConfirmModal } from "../../../../shared/components/ConfirmModal";
import { updatePoi, deletePoi } from "../../../../shared/api/poiService";
import { buildPoiStyle } from "../hooks/usePoiLoader";

const wktFormat = new WKT();

interface PoiInfoPopupProps {
  feature:    Feature;
  categories: CategoryTreeNode[];
  userLookup: Map<number, UserLookupEntry>;
  canManage:  boolean;
  onClose:    () => void;
  onUpdated:  () => void;
  onDeleted:  () => void;
}

export function PoiInfoPopup({ feature, categories, userLookup, canManage, onClose, onUpdated, onDeleted }: PoiInfoPopupProps) {
  const { apiFetch } = useAuth();

  const [isEditing,    setIsEditing]    = useState(false);
  const [name,         setName]         = useState(feature.get("poiName") as string);
  const [workingHours, setWorkingHours] = useState(feature.get("poiWorkingHours") as string);
  const [categoryId,   setCategoryId]   = useState<number>(feature.get("poiCategoryId") as number);
  const [showConfirm,  setShowConfirm]  = useState(false);
  const [isSaving,     setIsSaving]     = useState(false);
  const [isDeleting,   setIsDeleting]   = useState(false);
  const [error,        setError]        = useState<string | null>(null);

  const userId      = feature.get("poiUserId") as number;
  const createdDate = feature.get("poiCreatedDate") as string;
  const poiId       = feature.get("poiId") as number;

  const path            = findCategoryPath(categories, feature.get("poiCategoryId") as number);
  const categoryLabel   = path ? path.join(" > ") : "—";
  const creatorEmail    = userLookup.get(userId)?.email ?? "Bilinmiyor";
  const categoryOptions = flattenCategories(categories);

  async function handleSave() {
    setIsSaving(true);
    setError(null);
    try {
      const geometry = feature.getGeometry();
      if (!geometry) return;
      const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");
      const wktGeometry = wktFormat.writeGeometry(cloned);

      const res = await updatePoi(apiFetch, poiId, { name, workingHours, categoryId, wktGeometry });

      if (!res.ok) {
        setError(res.status === 403 ? "Bu POI'yi düzenleme yetkiniz yok." : "Güncelleme başarısız.");
        return;
      }

      feature.set("poiName", name);
      feature.set("poiWorkingHours", workingHours);
      feature.set("poiCategoryId", categoryId);
      feature.setStyle(buildPoiStyle(categoryId));

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
      const res = await deletePoi(apiFetch, poiId);
      if (!res.ok) {
        setError(res.status === 403 ? "Bu POI'yi silme yetkiniz yok." : "Silme başarısız.");
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
          POI
        </span>

        {!isEditing ? (
          <>
            <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "4px 0 20px" }}>
              {feature.get("poiName") as string}
            </h2>

            <InfoRow label="Kategori"          value={categoryLabel} />
            <InfoRow label="Mesai Saatleri"    value={feature.get("poiWorkingHours") as string} />
            <InfoRow label="Ekleyen Kullanıcı" value={creatorEmail} />
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
              POI Düzenle
            </h2>

            <div style={{ marginBottom: 14 }}>
              <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
                POI İsmi
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

            <div style={{ marginBottom: 14 }}>
              <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
                Kategori
              </label>
              <select
                value={categoryId}
                onChange={(e) => setCategoryId(Number(e.target.value))}
                style={{
                  width: "100%", boxSizing: "border-box", padding: "8px 12px",
                  borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
                  color: "#0f172a", outline: "none", background: "#ffffff",
                }}
              >
                {categoryOptions.map((o) => (
                  <option key={o.id} value={o.id} disabled={o.depth === 0}>
                    {o.depth === 0 ? o.name : `${"— ".repeat(o.depth)}${o.name}`}
                  </option>
                ))}
              </select>
            </div>

            <div style={{ marginBottom: 20 }}>
              <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
                Mesai Saatleri
              </label>
              <input
                type="text"
                value={workingHours}
                onChange={(e) => setWorkingHours(e.target.value)}
                placeholder="09:00 - 18:00"
                style={{
                  width: "100%", boxSizing: "border-box", padding: "8px 12px",
                  borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
                  color: "#0f172a", outline: "none",
                }}
              />
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
          message={`"${feature.get("poiName") as string}" POI'sini silmek istediğinize emin misiniz?`}
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
