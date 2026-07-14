import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { CategoryTreeManager } from "../components/CategoryTreeManager";
import { flattenCategories, type CategoryTreeNode } from "../../../shared/utils/categoryTree";

interface PoiDto {
  id:           number;
  name:         string;
  workingHours: string;
  wktGeometry:  string;
  categoryId:   number;
  userId:       number;
  createdDate:  string;
}

interface UserLookupEntry {
  id:    number;
  email: string;
}

export function PoiManagement() {
  const [pois,       setPois]       = useState<PoiDto[]>([]);
  const [categories, setCategories] = useState<CategoryTreeNode[]>([]);
  const [users,      setUsers]      = useState<UserLookupEntry[]>([]);
  const [isLoading,  setIsLoading]  = useState(true);
  const [error,      setError]      = useState<string | null>(null);

  const { apiFetch } = useAuth();

  async function fetchAll() {
    setIsLoading(true);
    setError(null);
    try {
      const [poiRes, catRes, userRes] = await Promise.all([
        apiFetch("/api/poi"),
        apiFetch("/api/poi-category/tree"),
        apiFetch("/api/users/lookup"),
      ]);

      if (!poiRes.ok || !catRes.ok || !userRes.ok) {
        setError("Veriler yüklenemedi.");
        return;
      }

      setPois(await poiRes.json());
      setCategories(await catRes.json());
      setUsers(await userRes.json());
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => { fetchAll(); }, []);

  const categoryNameById = new Map(flattenCategories(categories).map((c) => [c.id, c.name]));
  const emailById        = new Map(users.map((u) => [u.id, u.email]));

  return (
    <div>
      <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
        POI Yönetimi
      </h1>
      <p style={{ fontSize: 14, color: "#64748b", marginBottom: 24 }}>
        Sistemdeki ilgi noktalarını (POI) ve kategori hiyerarşisini yönetin.
      </p>

      {isLoading && <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>}
      {error     && <p style={{ color: "#ef4444", fontSize: 13 }}>{error}</p>}

      {!isLoading && !error && (
        <>
          <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden", marginBottom: 32 }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
              <thead>
                <tr style={{ background: "#f8fafc" }}>
                  <th style={thStyle}>ID</th>
                  <th style={thStyle}>İsim</th>
                  <th style={thStyle}>Kategori</th>
                  <th style={thStyle}>Mesai Saatleri</th>
                  <th style={thStyle}>Ekleyen Kullanıcı</th>
                  <th style={thStyle}>Oluşturma Tarihi</th>
                </tr>
              </thead>
              <tbody>
                {pois.length === 0 ? (
                  <tr>
                    <td colSpan={6} style={{ padding: 24, textAlign: "center", color: "#94a3b8", fontSize: 13 }}>
                      Henüz POI eklenmemiş.
                    </td>
                  </tr>
                ) : (
                  pois.map((poi) => (
                    <tr key={poi.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                      <td style={tdStyle}>{poi.id}</td>
                      <td style={tdStyle}>{poi.name}</td>
                      <td style={tdStyle}>{categoryNameById.get(poi.categoryId) ?? "—"}</td>
                      <td style={tdStyle}>{poi.workingHours}</td>
                      <td style={tdStyle}>{emailById.get(poi.userId) ?? "—"}</td>
                      <td style={tdStyle}>{new Date(poi.createdDate).toLocaleString("tr-TR")}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          <CategoryTreeManager categories={categories} onChanged={fetchAll} />
        </>
      )}
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding:       "10px 16px",
  textAlign:     "left",
  fontSize:      11,
  fontWeight:    600,
  color:         "#64748b",
  letterSpacing: ".5px",
};

const tdStyle: React.CSSProperties = {
  padding: "10px 16px",
  color:   "#374151",
};