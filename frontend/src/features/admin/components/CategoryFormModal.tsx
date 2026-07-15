import { useState } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { flattenCategories, type CategoryTreeNode, type FlatCategoryRow } from "../../../shared/utils/categoryTree";
import { createCategory, updateCategory } from "../api/categoryService";

interface CategoryFormModalProps {
  categories: CategoryTreeNode[];
  editTarget: FlatCategoryRow | null;
  onClose:    () => void;
  onSaved:    () => void;
}

function collectSubtreeIds(nodes: CategoryTreeNode[], targetId: number): Set<number> {
  const ids = new Set<number>();
  function visit(node: CategoryTreeNode, insideTarget: boolean) {
    const isTarget = insideTarget || node.id === targetId;
    if (isTarget) ids.add(node.id);
    node.children.forEach((c) => visit(c, isTarget));
  }
  nodes.forEach((n) => visit(n, false));
  return ids;
}

export function CategoryFormModal({ categories, editTarget, onClose, onSaved }: CategoryFormModalProps) {
  const [name,     setName]     = useState(editTarget?.name ?? "");
  const [parentId, setParentId] = useState<number | "">(editTarget?.parentId ?? "");
  const [isSaving, setIsSaving] = useState(false);
  const [error,    setError]    = useState<string | null>(null);

  const { apiFetch } = useAuth();

  const excludedIds = editTarget ? collectSubtreeIds(categories, editTarget.id) : new Set<number>();
  const options = flattenCategories(categories).filter((c) => !excludedIds.has(c.id));

  async function handleSave() {
    if (!name.trim()) { setError("Kategori adı zorunludur."); return; }
    setIsSaving(true);
    setError(null);

    const payload = {
      name:             name.trim(),
      parentCategoryId: parentId === "" ? null : parentId,
    };

    try {
      const res = editTarget
        ? await updateCategory(apiFetch, editTarget.id, payload)
        : await createCategory(apiFetch, payload);

      if (!res.ok) {
        const message = await res.text();
        setError(message || "Kaydedilemedi.");
        return;
      }
      onSaved();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, zIndex: 3000,
        background: "rgba(0,0,0,0.4)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "28px 32px", width: 380,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 20px" }}>
          {editTarget ? "Kategoriyi Düzenle" : "Yeni Kategori"}
        </h2>

        <div style={{ marginBottom: 16 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Kategori Adı
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

        <div style={{ marginBottom: 24 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Üst Kategori
          </label>
          <select
            value={parentId}
            onChange={(e) => setParentId(e.target.value === "" ? "" : Number(e.target.value))}
            style={{
              width: "100%", boxSizing: "border-box", padding: "8px 12px",
              borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
              color: "#0f172a", outline: "none", background: "#ffffff",
            }}
          >
            <option value="">— Ana Kategori (üst kategori yok) —</option>
            {options.map((o) => (
              <option key={o.id} value={o.id}>
                {"— ".repeat(o.depth)}{o.name}
              </option>
            ))}
          </select>
        </div>

        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 16px" }}>{error}</p>}

        <div style={{ display: "flex", gap: 10 }}>
          <button
            onClick={onClose}
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
            disabled={isSaving}
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
      </div>
    </div>
  );
}