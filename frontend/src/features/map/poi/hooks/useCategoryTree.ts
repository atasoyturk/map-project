import { useEffect, useState } from "react";
import type { CategoryTreeNode } from "../../../../shared/utils/categoryTree";
import { getCategoryTree } from "../../../../shared/api/poiService";

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

export function useCategoryTree(apiFetch: ApiFetch): CategoryTreeNode[] {
  const [tree, setTree] = useState<CategoryTreeNode[]>([]);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      try {
        const res = await getCategoryTree(apiFetch);
        if (!res.ok) return;
        const data: CategoryTreeNode[] = await res.json();
        if (!cancelled) setTree(data);
      } catch {  }
    }

    load();
    return () => { cancelled = true; };
  }, [apiFetch]);

  return tree;
}