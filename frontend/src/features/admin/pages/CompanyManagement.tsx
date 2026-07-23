import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import {
  getAllCompanyCategories, createCompanyCategory, deleteCompanyCategory,
  getAllCompanies, createCompany, updateCompany, deleteCompany,
  getAllVehicles, createVehicle, deleteVehicle,
} from "../../../shared/api/companyService";
import type { CompanyCategoryResponseDto, CompanyResponseDto, VehicleResponseDto } from "../../company/types";
import { CompanyRoutesModal } from "../components/CompanyRoutesModal";
import { getCompanyStats } from "../../../shared/api/companyService";
import { ShipmentRecordsModal } from "../../company/components/ShipmentRecordsModal";
import type { CompanyStatsDto } from "../../company/types";

export function CompanyManagement() {
  const [categories,      setCategories]      = useState<CompanyCategoryResponseDto[]>([]);
  const [newCategoryName, setNewCategoryName]  = useState("");
  const [isLoading,       setIsLoading]        = useState(true);
  const [isSavingCategory,setIsSavingCategory]  = useState(false);
  const [error,           setError]            = useState<string | null>(null);

  const [companies,         setCompanies]         = useState<CompanyResponseDto[]>([]);
  const [newCompanyName,    setNewCompanyName]    = useState("");
  const [newCompanyCategoryId, setNewCompanyCategoryId] = useState<number | "">("");
  const [isSavingCompany,   setIsSavingCompany]   = useState(false);
  const [editingCompanyId,  setEditingCompanyId]  = useState<number | null>(null);
  const [editCompanyName,   setEditCompanyName]   = useState("");
  const [editCompanyCategoryId, setEditCompanyCategoryId] = useState<number | "">("");
  const [routesModalCompany, setRoutesModalCompany] = useState<CompanyResponseDto | null>(null);
  const [stats, setStats] = useState<CompanyStatsDto[]>([]);
  const [showShipmentsModal, setShowShipmentsModal] = useState(false);

  async function fetchStats() {
    try {
      const res = await getCompanyStats(apiFetch);
      if (!res.ok) return;
      setStats(await res.json());
    } catch { }
  }

  const [vehicles,          setVehicles]          = useState<VehicleResponseDto[]>([]);
  const [newPlateNumber,    setNewPlateNumber]    = useState("");
  const [newVehicleCompanyId, setNewVehicleCompanyId] = useState<number | "">("");
  const [isSavingVehicle,   setIsSavingVehicle]   = useState(false);

  const { apiFetch } = useAuth();

  async function fetchCategories() {
    setIsLoading(true);
    try {
      const res = await getAllCompanyCategories(apiFetch);
      if (!res.ok) { setError("Kategoriler yüklenemedi."); return; }
      setCategories(await res.json());
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  async function fetchCompanies() {
    try {
      const res = await getAllCompanies(apiFetch);
      if (!res.ok) return;
      setCompanies(await res.json());
    } catch { }
  }

  useEffect(() => { fetchCategories(); fetchCompanies(); fetchVehicles(); fetchStats(); }, []);

  async function fetchVehicles() {
    try {
      const res = await getAllVehicles(apiFetch);
      if (!res.ok) return;
      setVehicles(await res.json());
    } catch { }
  }

  async function handleCreateCompany() {
    if (!newCompanyName.trim() || newCompanyCategoryId === "") return;
    setIsSavingCompany(true);
    try {
      const res = await createCompany(apiFetch, {
        name: newCompanyName.trim(), companyCategoryId: newCompanyCategoryId,
      });
      if (!res.ok) { setError("Şirket oluşturulamadı."); return; }
      setNewCompanyName("");
      setNewCompanyCategoryId("");
      fetchCompanies();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSavingCompany(false);
    }
  }

  function startEditCompany(company: CompanyResponseDto) {
    setEditingCompanyId(company.id);
    setEditCompanyName(company.name);
    setEditCompanyCategoryId(company.companyCategoryId);
  }

  async function handleUpdateCompany() {
    if (editingCompanyId === null || !editCompanyName.trim() || editCompanyCategoryId === "") return;
    try {
      const res = await updateCompany(apiFetch, editingCompanyId, {
        name: editCompanyName.trim(), companyCategoryId: editCompanyCategoryId,
      });
      if (!res.ok) { setError("Şirket güncellenemedi."); return; }
      setEditingCompanyId(null);
      fetchCompanies();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    }
  }

  async function handleDeleteCompany(id: number) {
    if (!confirm("Bu şirketi silmek istediğinize emin misiniz?")) return;
    try {
      const res = await deleteCompany(apiFetch, id);
      if (!res.ok) {
        const message = res.status === 409 ? await res.text() : "Şirket silinemedi.";
        setError(message);
        return;
      }
      fetchCompanies();
    } catch { }
  }

  async function handleCreateVehicle() {
    if (!newPlateNumber.trim() || newVehicleCompanyId === "") return;
    setIsSavingVehicle(true);
    try {
      const res = await createVehicle(apiFetch, {
        plateNumber: newPlateNumber.trim(), companyId: newVehicleCompanyId,
      });
      if (!res.ok) {
        const message = res.status === 409 ? await res.text() : "Araç eklenemedi.";
        setError(message);
        return;
      }
      setNewPlateNumber("");
      setNewVehicleCompanyId("");
      fetchVehicles();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSavingVehicle(false);
    }
  }

  async function handleDeleteVehicle(id: number) {
    if (!confirm("Bu aracı silmek istediğinize emin misiniz?")) return;
    try {
      const res = await deleteVehicle(apiFetch, id);
      if (!res.ok) { setError("Araç silinemedi."); return; }
      fetchVehicles();
    } catch { }
  }

  async function handleCreateCategory() {
    if (!newCategoryName.trim()) return;
    setIsSavingCategory(true);
    try {
      const res = await createCompanyCategory(apiFetch, newCategoryName.trim());
      if (!res.ok) return;
      setNewCategoryName("");
      fetchCategories();
    } catch { }
    finally { setIsSavingCategory(false); }
  }

  async function handleDeleteCategory(id: number) {
    if (!confirm("Bu kategoriyi silmek istediğinize emin misiniz?")) return;
    try {
      const res = await deleteCompanyCategory(apiFetch, id);
      if (!res.ok) {
        const message = res.status === 409 ? await res.text() : "Kategori silinemedi.";
        setError(message);
        return;
      }
      fetchCategories();
    } catch { }
  }

  return (
    <div>
      <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
        Hizmet Yönetimi
      </h1>
      <p style={{ fontSize: 14, color: "#64748b", marginBottom: 24 }}>
        Şirket kategorilerini, şirketleri ve araç filolarını yönetin.
      </p>

      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
        <div style={{ display: "flex", gap: 24 }}>
          <SummaryStat label="Kategori"  value={categories.length} />
          <SummaryStat label="Şirket"    value={companies.length} />
          <SummaryStat label="Araç"      value={vehicles.length} />
          <SummaryStat
            label="Tamamlanan Sevkiyat"
            value={stats.reduce((sum, s) => sum + s.completedShipmentCount, 0)}
          />
        </div>
        <button
          onClick={() => setShowShipmentsModal(true)}
          style={{
            padding: "8px 16px", borderRadius: 8,
            border: "1px solid rgba(16,185,129,.3)", background: "rgba(16,185,129,.05)",
            color: "#10b981", fontSize: 13, fontWeight: 500, cursor: "pointer",
          }}
        >
          Tamamlanan Sevkiyatları Görüntüle
        </button>
      </div>

      {error && <p style={{ color: "#ef4444", fontSize: 13, marginBottom: 16 }}>{error}</p>}

      <h2 style={{ fontSize: 16, fontWeight: 600, color: "#0f172a", marginBottom: 12 }}>
        Şirket Kategorileri
      </h2>

      <div style={{
        background: "#ffffff", borderRadius: 12,
        border: "1px solid #e2e8f0", padding: 16,
        marginBottom: 16, display: "flex", gap: 8,
      }}>
        <input
          type="text"
          placeholder="Yeni kategori adı..."
          value={newCategoryName}
          onChange={(e) => setNewCategoryName(e.target.value)}
          style={{
            flex: 1, padding: "8px 12px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13,
            color: "#0f172a", outline: "none",
          }}
        />
        <button
          onClick={handleCreateCategory}
          disabled={isSavingCategory || !newCategoryName.trim()}
          style={{
            padding: "8px 16px", borderRadius: 8, border: "none",
            background: "#0f172a", color: "#ffffff", fontSize: 13, fontWeight: 500,
            cursor: isSavingCategory ? "not-allowed" : "pointer",
            opacity: isSavingCategory ? 0.6 : 1,
          }}
        >
          Ekle
        </button>
      </div>

      {isLoading ? (
        <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>
      ) : (
        <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden", marginBottom: 32 }}>
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
            <thead>
              <tr style={{ background: "#f8fafc" }}>
                <th style={thStyle}>ID</th>
                <th style={thStyle}>Kategori Adı</th>
                <th style={thStyle}>İşlem</th>
              </tr>
            </thead>
            <tbody>
              {categories.map((category) => (
                <tr key={category.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                  <td style={tdStyle}>{category.id}</td>
                  <td style={tdStyle}>{category.name}</td>
                  <td style={tdStyle}>
                    <button
                      onClick={() => handleDeleteCategory(category.id)}
                      style={{
                        padding: "4px 10px", borderRadius: 6,
                        border: "1px solid rgba(239,68,68,.3)", background: "rgba(239,68,68,.05)",
                        color: "#ef4444", fontSize: 12, cursor: "pointer",
                      }}
                    >
                      Sil
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <h2 style={{ fontSize: 16, fontWeight: 600, color: "#0f172a", marginBottom: 12 }}>
        Şirketler
      </h2>

      <div style={{
        background: "#ffffff", borderRadius: 12,
        border: "1px solid #e2e8f0", padding: 16,
        marginBottom: 16, display: "flex", gap: 8,
      }}>
        <input
          type="text"
          placeholder="Yeni şirket adı..."
          value={newCompanyName}
          onChange={(e) => setNewCompanyName(e.target.value)}
          style={{
            flex: 1, padding: "8px 12px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13,
            color: "#0f172a", outline: "none",
          }}
        />
        <select
          value={newCompanyCategoryId}
          onChange={(e) => setNewCompanyCategoryId(e.target.value === "" ? "" : Number(e.target.value))}
          style={{
            padding: "8px 12px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13, color: "#0f172a",
          }}
        >
          <option value="">— Kategori seçin —</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
        <button
          onClick={handleCreateCompany}
          disabled={isSavingCompany || !newCompanyName.trim() || newCompanyCategoryId === ""}
          style={{
            padding: "8px 16px", borderRadius: 8, border: "none",
            background: "#0f172a", color: "#ffffff", fontSize: 13, fontWeight: 500,
            cursor: isSavingCompany ? "not-allowed" : "pointer",
            opacity: isSavingCompany ? 0.6 : 1,
          }}
        >
          Ekle
        </button>
      </div>

      <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden", marginBottom: 32 }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
          <thead>
            <tr style={{ background: "#f8fafc" }}>
              <th style={thStyle}>ID</th>
              <th style={thStyle}>Şirket Adı</th>
              <th style={thStyle}>Kategori</th>
              <th style={thStyle}>İşlem</th>
            </tr>
          </thead>
          <tbody>
            {companies.map((company) => (
              <tr key={company.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                <td style={tdStyle}>{company.id}</td>
                <td style={tdStyle}>
                  {editingCompanyId === company.id ? (
                    <input
                      type="text"
                      value={editCompanyName}
                      onChange={(e) => setEditCompanyName(e.target.value)}
                      style={{ padding: "4px 8px", borderRadius: 6, border: "1px solid #e2e8f0", fontSize: 12 }}
                    />
                  ) : company.name}
                </td>
                <td style={tdStyle}>
                  {editingCompanyId === company.id ? (
                    <select
                      value={editCompanyCategoryId}
                      onChange={(e) => setEditCompanyCategoryId(Number(e.target.value))}
                      style={{ padding: "4px 8px", borderRadius: 6, border: "1px solid #e2e8f0", fontSize: 12 }}
                    >
                      {categories.map((c) => (
                        <option key={c.id} value={c.id}>{c.name}</option>
                      ))}
                    </select>
                  ) : company.companyCategoryName}
                </td>
                <td style={tdStyle}>
                  {editingCompanyId === company.id ? (
                    <div style={{ display: "flex", gap: 6 }}>
                      <button
                        onClick={handleUpdateCompany}
                        style={{
                          padding: "4px 10px", borderRadius: 6, border: "none",
                          background: "#0f172a", color: "#ffffff", fontSize: 12, cursor: "pointer",
                        }}
                      >
                        Kaydet
                      </button>
                      <button
                        onClick={() => setEditingCompanyId(null)}
                        style={{
                          padding: "4px 10px", borderRadius: 6,
                          border: "1px solid #e2e8f0", background: "#f8fafc",
                          color: "#64748b", fontSize: 12, cursor: "pointer",
                        }}
                      >
                        İptal
                      </button>
                    </div>
                  ) : (
                    <div style={{ display: "flex", gap: 6 }}>
                      <button
                        onClick={() => setRoutesModalCompany(company)}
                        style={{
                          padding: "4px 10px", borderRadius: 6,
                          border: "1px solid rgba(16,185,129,.3)", background: "rgba(16,185,129,.05)",
                          color: "#10b981", fontSize: 12, cursor: "pointer",
                        }}
                      >
                        Güzergahlar
                      </button>
                      <button
                        onClick={() => startEditCompany(company)}
                        style={{
                          padding: "4px 10px", borderRadius: 6,
                          border: "1px solid rgba(59,130,246,.3)", background: "rgba(59,130,246,.05)",
                          color: "#3b82f6", fontSize: 12, cursor: "pointer",
                        }}
                      >
                        Düzenle
                      </button>
                      <button
                        onClick={() => handleDeleteCompany(company.id)}
                        style={{
                          padding: "4px 10px", borderRadius: 6,
                          border: "1px solid rgba(239,68,68,.3)", background: "rgba(239,68,68,.05)",
                          color: "#ef4444", fontSize: 12, cursor: "pointer",
                        }}
                      >
                        Sil
                      </button>
                    </div>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {stats.length > 0 && (
        <div style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 16, fontWeight: 600, color: "#0f172a", marginBottom: 12 }}>
            Şirket Bazlı Özet
          </h2>
          <div style={{
            display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(220px, 1fr))", gap: 12,
          }}>
            {stats.map((s) => (
              <div
                key={s.companyId}
                style={{
                  background: "#ffffff", borderRadius: 10,
                  border: "1px solid #e2e8f0", padding: 14,
                }}
              >
                <p style={{ fontSize: 13, fontWeight: 600, color: "#0f172a", margin: "0 0 8px" }}>
                  {s.companyName}
                </p>
                <p style={{ fontSize: 12, color: "#64748b", margin: "2px 0" }}>Araç: {s.vehicleCount}</p>
                <p style={{ fontSize: 12, color: "#64748b", margin: "2px 0" }}>Güzergah: {s.routeCount}</p>
                <p style={{ fontSize: 12, color: "#64748b", margin: "2px 0" }}>
                  Tamamlanan Sevkiyat: {s.completedShipmentCount}
                </p>
              </div>
            ))}
          </div>
        </div>
      )}

      <h2 style={{ fontSize: 16, fontWeight: 600, color: "#0f172a", marginBottom: 12 }}>
        Araç Filosu
      </h2>

      <div style={{
        background: "#ffffff", borderRadius: 12,
        border: "1px solid #e2e8f0", padding: 16,
        marginBottom: 16, display: "flex", gap: 8,
      }}>
        <input
          type="text"
          placeholder="Plaka (örn. 34 ABC 123)"
          value={newPlateNumber}
          onChange={(e) => setNewPlateNumber(e.target.value)}
          style={{
            flex: 1, padding: "8px 12px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13,
            color: "#0f172a", outline: "none",
          }}
        />
        <select
          value={newVehicleCompanyId}
          onChange={(e) => setNewVehicleCompanyId(e.target.value === "" ? "" : Number(e.target.value))}
          style={{
            padding: "8px 12px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13, color: "#0f172a",
          }}
        >
          <option value="">— Şirket seçin —</option>
          {companies.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
        <button
          onClick={handleCreateVehicle}
          disabled={isSavingVehicle || !newPlateNumber.trim() || newVehicleCompanyId === ""}
          style={{
            padding: "8px 16px", borderRadius: 8, border: "none",
            background: "#0f172a", color: "#ffffff", fontSize: 13, fontWeight: 500,
            cursor: isSavingVehicle ? "not-allowed" : "pointer",
            opacity: isSavingVehicle ? 0.6 : 1,
          }}
        >
          Ekle
        </button>
      </div>

      <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden", marginBottom: 32 }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
          <thead>
            <tr style={{ background: "#f8fafc" }}>
              <th style={thStyle}>ID</th>
              <th style={thStyle}>Plaka</th>
              <th style={thStyle}>Şirket</th>
              <th style={thStyle}>İşlem</th>
            </tr>
          </thead>
          <tbody>
            {vehicles.map((vehicle) => (
              <tr key={vehicle.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                <td style={tdStyle}>{vehicle.id}</td>
                <td style={tdStyle}>{vehicle.plateNumber}</td>
                <td style={tdStyle}>{vehicle.companyName}</td>
                <td style={tdStyle}>
                  <button
                    onClick={() => handleDeleteVehicle(vehicle.id)}
                    style={{
                      padding: "4px 10px", borderRadius: 6,
                      border: "1px solid rgba(239,68,68,.3)", background: "rgba(239,68,68,.05)",
                      color: "#ef4444", fontSize: 12, cursor: "pointer",
                    }}
                  >
                    Sil
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {routesModalCompany && (
        <CompanyRoutesModal
          companyId={routesModalCompany.id}
          companyName={routesModalCompany.name}
          onClose={() => setRoutesModalCompany(null)}
        />
      )}

      {showShipmentsModal && (
        <ShipmentRecordsModal onClose={() => setShowShipmentsModal(false)} />
      )}
    </div>
  );
}

function SummaryStat({ label, value }: { label: string; value: number }) {
  return (
    <div>
      <p style={{ fontSize: 20, fontWeight: 700, color: "#0f172a", margin: 0 }}>{value}</p>
      <p style={{ fontSize: 12, color: "#94a3b8", margin: 0 }}>{label}</p>
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding: "10px 16px", textAlign: "left",
  fontSize: 11, fontWeight: 600, color: "#64748b", letterSpacing: ".5px",
};

const tdStyle: React.CSSProperties = {
  padding: "10px 16px", color: "#374151",
};