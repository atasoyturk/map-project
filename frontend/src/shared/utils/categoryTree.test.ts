import { describe, expect, it } from "vitest";
import { findCategoryPath, flattenCategories, type CategoryTreeNode } from "./categoryTree";

const tree: CategoryTreeNode[] = [
  {
    id: 1, name: "Yeme-İçme", children: [
      { id: 2, name: "Restoran", children: [{ id: 3, name: "Pizza", children: [] }] },
      { id: 4, name: "Kafe", children: [] },
    ],
  },
  { id: 5, name: "Market", children: [] },
];

describe("flattenCategories", () => {
  it("lists nodes depth-first with depth and parentId", () => {
    expect(flattenCategories(tree)).toEqual([
      { id: 1, name: "Yeme-İçme", depth: 0, parentId: null },
      { id: 2, name: "Restoran",  depth: 1, parentId: 1 },
      { id: 3, name: "Pizza",     depth: 2, parentId: 2 },
      { id: 4, name: "Kafe",      depth: 1, parentId: 1 },
      { id: 5, name: "Market",    depth: 0, parentId: null },
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
