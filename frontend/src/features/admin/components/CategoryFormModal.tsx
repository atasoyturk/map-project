import { useState } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { flattenCategories, type CategoryTreeNode, type FlatCategoryRow } from "../../../shared/utils/categoryTree";
import { createCategory, updateCategory } from "../api/categoryService";
import { Modal }  from "../../../shared/components/ui/Modal";
import { Button } from "../../../shared/components/ui/Button";
import { Field }  from "../../../shared/components/ui/Field";

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
  const [icon,     setIcon]     = useState(editTarget?.icon ?? "");
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
      icon:             icon.trim() || null,
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
    <Modal onClose={onClose} className="w-[380px] rounded-2xl px-8 py-7">
      <h2 className="mt-0 mb-5 text-lg font-semibold text-ink">
        {editTarget ? "Kategoriyi Düzenle" : "Yeni Kategori"}
      </h2>

      <Field label="Kategori Adı" className="mb-4">
        <input type="text" value={name} onChange={(e) => setName(e.target.value)} />
      </Field>

      <Field label="Üst Kategori" className="mb-4">
        <select
          value={parentId}
          onChange={(e) => setParentId(e.target.value === "" ? "" : Number(e.target.value))}
          className="w-full rounded-lg border border-line bg-white px-3 py-2 text-sm text-ink outline-none focus:border-indigo-500"
        >
          <option value="">— Ana Kategori (üst kategori yok) —</option>
          {options.map((o) => (
            <option key={o.id} value={o.id}>
              {"— ".repeat(o.depth)}{o.name}
            </option>
          ))}
        </select>
      </Field>

      <Field label="İkon (emoji)" className="mb-6">
        <input
          type="text"
          value={icon}
          maxLength={16}
          placeholder="Boş bırakılırsa üst kategorinin ikonu kullanılır"
          onChange={(e) => setIcon(e.target.value)}
        />
      </Field>

      {error && <p className="mt-0 mb-4 text-xs text-red-600">{error}</p>}

      <div className="flex gap-2.5">
        <Button className="flex-1" onClick={onClose} disabled={isSaving}>İptal</Button>
        <Button
          className={`flex-2 ${isSaving ? "bg-slate-800! text-muted!" : ""}`}
          variant="primary"
          onClick={handleSave}
          disabled={isSaving}
        >
          {isSaving ? "Kaydediliyor..." : "Kaydet"}
        </Button>
      </div>
    </Modal>
  );
}
