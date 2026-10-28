import type { ApiFetch } from "../../../shared/api/apiFetch";

interface CategoryPayload {
  name:             string;
  parentCategoryId: number | null;
  icon:             string | null;
}

export function createCategory(apiFetch: ApiFetch, data: CategoryPayload) {
  return apiFetch("/api/poi-category", {
    method: "POST",
    body:   JSON.stringify(data),
  });
}

export function updateCategory(apiFetch: ApiFetch, id: number, data: CategoryPayload) {
  return apiFetch(`/api/poi-category/${id}`, {
    method: "PUT",
    body:   JSON.stringify(data),
  });
}

export function deleteCategory(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/poi-category/${id}`, { method: "DELETE" });
}