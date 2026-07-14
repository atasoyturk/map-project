import type { Feature } from "ol";
import { findCategoryPath, type CategoryTreeNode } from "../../../shared/utils/categoryTree";
import type { UserLookupEntry } from "../hooks/useUserLookup";

interface PoiInfoPopupProps {
  feature:    Feature;
  categories: CategoryTreeNode[];
  userLookup: Map<number, UserLookupEntry>;
  onClose:    () => void;
}

export function PoiInfoPopup({ feature, categories, userLookup, onClose }: PoiInfoPopupProps) {
  const name         = feature.get("poiName") as string;
  const workingHours = feature.get("poiWorkingHours") as string;
  const categoryId   = feature.get("poiCategoryId") as number;
  const userId       = feature.get("poiUserId") as number;
  const createdDate  = feature.get("poiCreatedDate") as string;

  const path            = findCategoryPath(categories, categoryId);
  const categoryLabel   = path ? path.join(" > ") : "—";
  const creatorEmail    = userLookup.get(userId)?.email ?? "Bilinmiyor";

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
          padding: "28px 32px", width: 340,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
        }}
      >
        <span style={{ fontSize: 11, fontWeight: 600, color: "#0d9488", letterSpacing: ".8px", textTransform: "uppercase" }}>
          POI
        </span>
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "4px 0 20px" }}>
          {name}
        </h2>

        <InfoRow label="Kategori"          value={categoryLabel} />
        <InfoRow label="Mesai Saatleri"    value={workingHours} />
        <InfoRow label="Ekleyen Kullanıcı" value={creatorEmail} />
        <InfoRow label="Oluşturma Tarihi"  value={new Date(createdDate).toLocaleString("tr-TR")} />

        <button
          onClick={onClose}
          style={{
            width: "100%", marginTop: 16, padding: "9px 0",
            borderRadius: 8, border: "1px solid #e2e8f0",
            background: "#f8fafc", color: "#64748b",
            fontSize: 13, cursor: "pointer",
          }}
        >
          Kapat
        </button>
      </div>
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