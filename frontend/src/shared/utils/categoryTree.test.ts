import { describe, expect, it } from "vitest";
import { buildCategoryIconMap, findCategoryPath, flattenCategories, type CategoryTreeNode } from "./categoryTree";

const tree: CategoryTreeNode[] = [
  {
    id: 1, name: "Yeme-İçme", icon: "🍽️", children: [
      { id: 2, name: "Restoran", icon: null, children: [{ id: 3, name: "Pizza", icon: "🍕", children: [] }] },
      { id: 4, name: "Kafe", icon: null, children: [] },
    ],
  },
  { id: 5, name: "Market", icon: null, children: [] },
];

describe("flattenCategories", () => {
  it("lists nodes depth-first with depth and parentId", () => {
    expect(flattenCategories(tree)).toEqual([
      { id: 1, name: "Yeme-İçme", icon: "🍽️", depth: 0, parentId: null },
      { id: 2, name: "Restoran",  icon: null, depth: 1, parentId: 1 },
      { id: 3, name: "Pizza",     icon: "🍕", depth: 2, parentId: 2 },
      { id: 4, name: "Kafe",      icon: null, depth: 1, parentId: 1 },
      { id: 5, name: "Market",    icon: null, depth: 0, parentId: null },
    ]);
  });

  it("returns an empty list for an empty tree", () => {
    expect(flattenCategories([])).toEqual([]);
  });
});

describe("findCategoryPath", () => {
  it("returns the root name for a root node", () => {
    expect(findCategoryPath(tree, 5)).toEqual(["Market"]);
  });

  it("returns the names from root to a nested node", () => {
    expect(findCategoryPath(tree, 3)).toEqual(["Yeme-İçme", "Restoran", "Pizza"]);
  });

  it("returns null for an unknown id", () => {
    expect(findCategoryPath(tree, 99)).toBeNull();
  });
});

describe("buildCategoryIconMap", () => {
  const icons = buildCategoryIconMap(tree);

  it("uses the top-level icon for sub-categories without their own", () => {
    expect(icons.get(2)).toBe("🍽️");
    expect(icons.get(4)).toBe("🍽️");
  });

  it("prefers a category's own icon over the inherited one", () => {
    expect(icons.get(3)).toBe("🍕");
  });

  it("leaves out categories with no icon in their path", () => {
    expect(icons.has(5)).toBe(false);
  });
});
