export interface CategoryTreeNode {
  id:       number;
  name:     string;
  icon:     string | null;
  children: CategoryTreeNode[];
}

export interface FlatCategoryRow {
  id:       number;
  name:     string;
  icon:     string | null;
  depth:    number;
  parentId: number | null;
}

export function flattenCategories(
  nodes:    CategoryTreeNode[],
  depth:    number = 0,
  parentId: number | null = null,
): FlatCategoryRow[] {
  return nodes.flatMap((n) => [
    { id: n.id, name: n.name, icon: n.icon, depth, parentId },
    ...flattenCategories(n.children, depth + 1, n.id),
  ]);
}

export function findCategoryPath(nodes: CategoryTreeNode[], targetId: number): string[] | null {
  for (const node of nodes) {
    if (node.id === targetId) return [node.name];
    const childPath = findCategoryPath(node.children, targetId);
    if (childPath) return [node.name, ...childPath];
  }
  return null;
}

// Maps each category id to its own icon, else its nearest ancestor's; categories without either are left out.
export function buildCategoryIconMap(
  nodes:         CategoryTreeNode[],
  inheritedIcon: string | null = null,
  result:        Map<number, string> = new Map(),
): Map<number, string> {
  for (const node of nodes) {
    const icon = node.icon || inheritedIcon;
    if (icon) result.set(node.id, icon);
    buildCategoryIconMap(node.children, icon, result);
  }
  return result;
}
