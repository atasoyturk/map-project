import { useState } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { CategoryFormModal } from "./CategoryFormModal";
import { flattenCategories, type CategoryTreeNode, type FlatCategoryRow } from "../utils/categoryTree";


interface CategoryTreeManagerProps {
  categories: CategoryTreeNode[];
  onChanged:  () => void;
}

export function CategoryTreeManager({ categories, onChanged }: CategoryTreeManagerProps) {
  const [showForm,   setShowForm]   = useState(false);
  const [editTarget, setEditTarget] = useState<FlatCategoryRow | null>(null);
  const [error,      setError]      = useState<string | null>(null);

  const { apiFetch } = useAuth();
  const rows = flattenCategories(categories);

  async function handleDelete(id: number) {
    if (!confirm("Bu kategoriyi silmek istediğinize emin misiniz?")) return;
    setError(null);
    try {
      const res = await apiFetch(`/api/poi-category/${id}`, { method: "DELETE" });
      if (!res.ok) {
        const message = await res.text();
        setError(message || "Kategori silinemedi.");
        return;
      }
      onChanged();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    }
  }

  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 16 }}>
        <h2 style={{ fontSize: 16, fontWeight: 600, color: "#0f172a", margin: 0 }}>
          Kategori Yönetimi
        </h2>
        <button
          onClick={() => { setEditTarget(null); setShowForm(true); }}
          style={{
            padding: "6px 14px", borderRadius: 8, border: "none",
            background: "#0f172a", color: "#94a3b8",
            fontSize: 13, fontWeight: 500, cursor: "pointer",
          }}
        >
          + Yeni Kategori
        </button>
      </div>

      {error && <p style={{ fontSize: 12, color: "#dc2626", marginBottom: 12 }}>{error}</p>}

      <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden" }}>
        {rows.length === 0 ? (
          <p style={{ padding: 24, textAlign: "center", color: "#94a3b8", fontSize: 13 }}>
            Henüz kategori tanımlanmamış.
          </p>
        ) : (
          rows.map((row, i) => (
            <div
              key={row.id}
              style={{
                display: "flex", alignItems: "center", justifyContent: "space-between",
                padding: "10px 16px", paddingLeft: 16 + row.depth * 24,
                borderTop: i === 0 ? undefined : "1px solid #f1f5f9",
              }}
            >
              <span style={{
                fontSize: 13,
                color: row.depth === 0 ? "#0f172a" : "#475569",
                fontWeight: row.depth === 0 ? 600 : 400,
              }}>
                {row.depth > 0 && "— "}{row.name}
              </span>
              <div style={{ display: "flex", gap: 6 }}>
                <button
                  onClick={() => { setEditTarget(row); setShowForm(true); }}
                  style={{
                    padding: "3px 10px", borderRadius: 6,
                    border: "1px solid rgba(59,130,246,.3)",
                    background: "rgba(59,130,246,.05)",
                    color: "#3b82f6", fontSize: 12, cursor: "pointer",
                  }}
                >
                  Düzenle
                </button>
                <button
                  onClick={() => handleDelete(row.id)}
                  style={{
                    padding: "3px 10px", borderRadius: 6,
                    border: "1px solid rgba(239,68,68,.3)",
                    background: "rgba(239,68,68,.05)",
                    color: "#ef4444", fontSize: 12, cursor: "pointer",
                  }}
                >
                  Sil
                </button>
              </div>
            </div>
          ))
        )}
      </div>

      {showForm && (
        <CategoryFormModal
          categories={categories}
          editTarget={editTarget}
          onClose={() => setShowForm(false)}
          onSaved={() => { setShowForm(false); onChanged(); }}
        />
      )}
    </div>
  );
}