import { useState } from "react";
import { flattenCategories, type CategoryTreeNode } from "../../../../shared/utils/categoryTree";

interface PoiFormModalProps {
  categories: CategoryTreeNode[];
  onSave:     (data: { name: string; workingHours: string; categoryId: number }) => void;
  onCancel:   () => void;
  isSaving:   boolean;
  error?:     string | null;
}

export function PoiFormModal({ categories, onSave, onCancel, isSaving, error }: PoiFormModalProps) {
  const [name,       setName]       = useState("");
  const [categoryId, setCategoryId] = useState<number | "">("");
  const [startTime,  setStartTime]  = useState("09:00");
  const [endTime,    setEndTime]    = useState("18:00");

  const options = flattenCategories(categories);
  const canSave = !!name.trim() && categoryId !== "" && !isSaving;

  function handleSave() {
    if (!canSave) return;
    onSave({ name: name.trim(), workingHours: `${startTime} - ${endTime}`, categoryId: categoryId as number });
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
          Yeni POI Ekle
        </h2>

        <div style={{ marginBottom: 16 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            POI İsmi
          </label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Örn: Merkez Kafe"
            style={{
              width: "100%", boxSizing: "border-box", padding: "8px 12px",
              borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
              color: "#0f172a", outline: "none",
            }}
          />
        </div>

        <div style={{ marginBottom: 16 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Kategori
          </label>
          <select
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value === "" ? "" : Number(e.target.value))}
            style={{
              width: "100%", boxSizing: "border-box", padding: "8px 12px",
              borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
              color: "#0f172a", outline: "none", background: "#ffffff",
            }}
          >
            <option value="">— Kategori seçin —</option>
            {options.map((o) => (
              <option key={o.id} value={o.id} disabled={o.depth === 0}>
                {o.depth === 0 ? o.name : `${"— ".repeat(o.depth)}${o.name}`}
              </option>
            ))}
          </select>
        </div>

        <div style={{ marginBottom: 24 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Mesai Saatleri
          </label>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <input
              type="time"
              value={startTime}
              onChange={(e) => setStartTime(e.target.value)}
              style={{ flex: 1, padding: "8px 10px", borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 13, color: "#0f172a", outline: "none" }}
            />
            <span style={{ color: "#94a3b8", fontSize: 13 }}>—</span>
            <input
              type="time"
              value={endTime}
              onChange={(e) => setEndTime(e.target.value)}
              style={{ flex: 1, padding: "8px 10px", borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 13, color: "#0f172a", outline: "none" }}
            />
          </div>
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