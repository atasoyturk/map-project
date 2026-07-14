export interface CategoryTreeNode {
  id:       number;
  name:     string;
  children: CategoryTreeNode[];
}

export interface FlatCategoryRow {
  id:       number;
  name:     string;
  depth:    number;
  parentId: number | null;
}

export function flattenCategories(
  nodes:    CategoryTreeNode[],
  depth:    number = 0,
  parentId: number | null = null,
): FlatCategoryRow[] {
  return nodes.flatMap((n) => [
    { id: n.id, name: n.name, depth, parentId },
    ...flattenCategories(n.children, depth + 1, n.id),
  ]);
}