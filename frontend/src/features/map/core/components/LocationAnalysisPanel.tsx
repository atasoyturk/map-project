import { useState } from "react";
import { flattenCategories, type FlatCategoryRow } from "../../../../shared/utils/categoryTree";
import GeoJSON from "ol/format/GeoJSON";
import WKT from "ol/format/WKT";
import { RegionSearch } from "../../../admin/components/RegionSearch";
import { simplifyToLimit } from "../../../admin/helpers/simplifyGeometry";
import type { NominatimResult } from "../../../admin/api/nominatim";


interface Criterion {
  categoryId: number;
  score: number;
}

interface LocationAnalysisPanelProps {
  categories: any[];
  onStart: (criteria: Criterion[]) => void;
  onCancel: () => void;
  isPolygonSelected: boolean;
  onSelectPolygon: () => void;
  onClear: () => void;
  hasResults: boolean;
  onRegionSelect: (wkt: string, displayName: string) => void;

}

export function LocationAnalysisPanel({
  categories,
  onStart,
  onCancel,
  isPolygonSelected,
  onSelectPolygon,
  onClear,
  hasResults,
  onRegionSelect,
}: LocationAnalysisPanelProps) {
  const [criteria, setCriteria] = useState<Criterion[]>([]);
  const flatCategories: FlatCategoryRow[] = flattenCategories(categories);

  const totalScore = criteria.reduce((sum, c) => sum + c.score, 0);

  const addCriterion = () => {
    if (criteria.length < 5) {
      setCriteria([...criteria, { categoryId: flatCategories[0]?.id || 0, score: 0 }]);
    }
  };

  const removeCriterion = (index: number) => {
    setCriteria(criteria.filter((_, i) => i !== index));
  };

  const updateCriterion = (index: number, field: keyof Criterion, value: number) => {
    const next = [...criteria];
    next[index] = { ...next[index], [field]: value };
    setCriteria(next);
  };

  const canStart = isPolygonSelected && criteria.length >= 2 && totalScore === 100;

  const [selectionMode, setSelectionMode] = useState<"draw" | "search">("draw");

  function handleRegionSelect(geojson: NominatimResult["geojson"], displayName: string) {
    const raw = new GeoJSON().readGeometry(geojson, {
      dataProjection:    "EPSG:4326",
      featureProjection: "EPSG:3857",
    });
    const geometry = simplifyToLimit(raw);
    const cloned   = geometry.clone().transform("EPSG:3857", "EPSG:4326");
    const wkt      = new WKT().writeGeometry(cloned);
    onRegionSelect(wkt, displayName);
  }


    return (
    <div style={{
      position: "fixed", top: 70, right: 20, width: 320,
      background: "#ffffff", borderRadius: 12, boxShadow: "0 10px 25px rgba(0,0,0,0.1)",
      padding: 20, zIndex: 1100, border: "1px solid #e2e8f0"
    }}>
      <h3 style={{ margin: "0 0 16px", fontSize: 16, fontWeight: 600, color: "#0f172a" }}>Konum Analizi</h3>

      {/* Hedef Bölge Seçimi */}
      <div style={{ marginBottom: 20 }}>
        <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#64748b", marginBottom: 8 }}>
          Taranacak Bölge
        </label>

        {/* Mod Seçimi */}
        <div style={{ display: "flex", gap: 6, marginBottom: 8 }}>
          <button
            onClick={() => setSelectionMode("search")}
            style={{
              flex: 1, padding: "6px", borderRadius: 6, fontSize: 12,
              border: "1px solid",
              borderColor: selectionMode === "search" ? "#0f172a" : "#e2e8f0",
              background:  selectionMode === "search" ? "#0f172a" : "white",
              color:       selectionMode === "search" ? "white" : "#64748b",
              cursor: "pointer",
            }}
          >
            Bölge Ara
          </button>
          <button
            onClick={() => setSelectionMode("draw")}
            style={{
              flex: 1, padding: "6px", borderRadius: 6, fontSize: 12,
              border: "1px solid",
              borderColor: selectionMode === "draw" ? "#0f172a" : "#e2e8f0",
              background:  selectionMode === "draw" ? "#0f172a" : "white",
              color:       selectionMode === "draw" ? "white" : "#64748b",
              cursor: "pointer",
            }}
          >
            Haritada Çiz
          </button>
        </div>

        {/* Bölge Ara Modu */}
        {selectionMode === "search" && (
          <RegionSearch onSelect={handleRegionSelect} />
        )}

        {/* Haritada Çiz Modu */}
        {selectionMode === "draw" && (
          <button
            onClick={onSelectPolygon}
            style={{
              width: "100%", padding: "8px", borderRadius: 8, border: "1px solid",
              borderColor: isPolygonSelected ? "#10b981" : "#e2e8f0",
              background:  isPolygonSelected ? "#ecfdf5" : "#f8fafc",
              color:       isPolygonSelected ? "#059669" : "#64748b",
              fontSize: 13, cursor: "pointer"
            }}
          >
            {isPolygonSelected ? "✓ Bölge Seçildi" : "Haritada Poligon Çiz"}
          </button>
        )}
      </div>

      {/* Kriter Seçimi */}
      <div style={{ marginBottom: 20 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 8 }}>
          <label style={{ fontSize: 12, fontWeight: 500, color: "#64748b" }}>Odak (En az 2, en fazla 5)</label>
          {criteria.length < 5 && (
            <button onClick={addCriterion} style={{ background: "none", border: "none", color: "#3b82f6", fontSize: 12, cursor: "pointer", fontWeight: 600 }}>
              + Ekle
            </button>
          )}
        </div>

        {criteria.map((c, i) => (
          <div key={i} style={{ display: "flex", gap: 8, marginBottom: 8, alignItems: "center" }}>
            <select
              value={c.categoryId}
              onChange={(e) => updateCriterion(i, "categoryId", Number(e.target.value))}
              style={{ flex: 2, padding: "6px", borderRadius: 6, border: "1px solid #e2e8f0", fontSize: 12 }}
            >
              {flatCategories.map((cat: FlatCategoryRow) => (
                <option key={cat.id} value={cat.id}>{"— ".repeat(cat.depth)}{cat.name}</option>
              ))}
            </select>
            <input
              type="number"
              placeholder="Puan"
              value={c.score || ""}
              onChange={(e) => updateCriterion(i, "score", Number(e.target.value))}
              style={{ flex: 1, padding: "6px", borderRadius: 6, border: "1px solid #e2e8f0", fontSize: 12, width: 50 }}
            />
            <button onClick={() => removeCriterion(i)} style={{ background: "none", border: "none", color: "#ef4444", cursor: "pointer" }}>×</button>
          </div>
        ))}
      </div>

      {/* Puan Toplamı */}
      <div style={{ padding: "12px", borderRadius: 8, background: totalScore === 100 ? "#f0fdf4" : "#fef2f2", marginBottom: 20 }}>
        <div style={{ display: "flex", justifyContent: "space-between", fontSize: 13, fontWeight: 600 }}>
          <span style={{ color: "#64748b" }}>Toplam Puan:</span>
          <span style={{ color: totalScore === 100 ? "#10b981" : "#ef4444" }}>{totalScore} / 100</span>
        </div>
      </div>

      {/* Aksiyon Butonları */}
      <div style={{ display: "flex", gap: 10, flexDirection: "column" }}>
        {hasResults ? (
          <button
            onClick={onClear}
            style={{
              width: "100%", padding: "10px", borderRadius: 8, border: "none",
              background: "#ef4444", color: "#ffffff", fontSize: 13, fontWeight: 600,
              cursor: "pointer"
            }}
          >
            Analizi Temizle
          </button>
        ) : (
          <div style={{ display: "flex", gap: 10 }}>
            <button onClick={onCancel} style={{ flex: 1, padding: "10px", borderRadius: 8, border: "1px solid #e2e8f0", background: "none", fontSize: 13, cursor: "pointer" }}>İptal</button>
            <button
              onClick={() => onStart(criteria)}
              disabled={!canStart}
              style={{
                flex: 2, padding: "10px", borderRadius: 8, border: "none",
                background: canStart ? "#0f172a" : "#94a3b8",
                color: "#ffffff", fontSize: 13, fontWeight: 600,
                cursor: canStart ? "pointer" : "not-allowed"
              }}
            >
              Analizi Başlat
            </button>
          </div>
        )}
      </div>
    </div>
  );

}
