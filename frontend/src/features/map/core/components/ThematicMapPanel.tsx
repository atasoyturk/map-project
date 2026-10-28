import { useState } from "react";
import { useAuth } from "../../../auth/context/AuthContext";
import { getIsochrone, type IsochroneResultDto } from "../api/thematicMapService";
import { Z } from "../../../../shared/components/ui/zIndex";

type PickMode = "center" | null;

const MAX_THRESHOLDS = 10;
const MIN_MINUTES = 1;
const MAX_MINUTES = 180;

interface ThematicMapPanelProps {
  onClose:              () => void;
  pickMode:             PickMode;
  onPickModeChange:     (mode: PickMode) => void;
  searchTarget:         PickMode;
  onSearchTargetChange: (target: PickMode) => void;
  centerPoint:          { lon: number; lat: number } | null;
  onResult:             (result: IsochroneResultDto | null) => void;
}

export function ThematicMapPanel({
  onClose, pickMode, onPickModeChange, searchTarget, onSearchTargetChange, centerPoint, onResult,
}: ThematicMapPanelProps) {
  const { apiFetch } = useAuth();

  const [thresholds,   setThresholds]   = useState<number[]>([15]);
  const [newThreshold, setNewThreshold] = useState("");
  const [isLoading,    setIsLoading]    = useState(false);
  const [error,        setError]        = useState<string | null>(null);
  const [hasResult,    setHasResult]    = useState(false);

  function handleAddThreshold() {
    const value = Number(newThreshold);
    if (!Number.isInteger(value) || value < MIN_MINUTES || value > MAX_MINUTES) {
      setError(`Süre ${MIN_MINUTES}-${MAX_MINUTES} dakika arasında bir tam sayı olmalıdır.`);
      return;
    }
    if (thresholds.includes(value)) {
      setError("Bu süre zaten eklenmiş.");
      return;
    }
    if (thresholds.length >= MAX_THRESHOLDS) {
      setError(`En fazla ${MAX_THRESHOLDS} süre eklenebilir.`);
      return;
    }

    setError(null);
    setThresholds((prev) => [...prev, value].sort((a, b) => a - b));
    setNewThreshold("");
  }

  function handleRemoveThreshold(value: number) {
    setThresholds((prev) => prev.filter((t) => t !== value));
  }

  async function handleCompute() {
    if (!centerPoint) return;

    // Kullanıcı bir süre yazıp "Ekle"ye basmadan doğrudan hesapla dediyse,
    // yazılı değeri de otomatik olarak listeye dahil ediyoruz.
    let effectiveThresholds = thresholds;
    const pendingValue = Number(newThreshold);
    if (
      newThreshold.trim() &&
      Number.isInteger(pendingValue) &&
      pendingValue >= MIN_MINUTES && pendingValue <= MAX_MINUTES &&
      !thresholds.includes(pendingValue) &&
      thresholds.length < MAX_THRESHOLDS
    ) {
      effectiveThresholds = [...thresholds, pendingValue].sort((a, b) => a - b);
      setThresholds(effectiveThresholds);
      setNewThreshold("");
    }

    if (effectiveThresholds.length === 0) {
      setError("En az bir süre eklemelisiniz.");
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const res = await getIsochrone(apiFetch, centerPoint.lon, centerPoint.lat, effectiveThresholds);
      if (!res.ok) { setError("Hizmet alanı hesaplanamadı."); return; }

      const data: IsochroneResultDto = await res.json();
      if (data.bands.length === 0) {
        setError("Bu nokta için hizmet alanı hesaplanamadı (yol ağı bulunamadı).");
        onResult(null);
        setHasResult(false);
        return;
      }

      onResult(data);
      setHasResult(true);
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div style={{
      position: "fixed", top: 70, left: 16, bottom: 100, width: 300,
      background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0",
      boxShadow: "0 8px 32px rgba(0,0,0,0.15)", zIndex: Z.sidePanel,
      display: "flex", flexDirection: "column", overflow: "hidden",
    }}>
      <div style={{
        display: "flex", alignItems: "center", justifyContent: "space-between",
        padding: "12px 16px", borderBottom: "1px solid #f1f5f9",
      }}>
        <span style={{ fontSize: 14, fontWeight: 600, color: "#0f172a" }}>Hizmet Alanı Haritası</span>
        <button
          onClick={onClose}
          style={{ border: "none", background: "transparent", color: "#94a3b8", fontSize: 16, cursor: "pointer" }}
        >
          ✕
        </button>
      </div>

      <div style={{ padding: 16, borderBottom: "1px solid #f1f5f9", display: "flex", flexDirection: "column", gap: 8 }}>
        <p style={{ fontSize: 12, color: "#64748b", margin: "0 0 4px" }}>
          Bir merkez nokta seçip, araçla kaç dakikada ulaşılabilecek alanları görmek istediğinizi belirtin.
        </p>

        <div style={{ display: "flex", alignItems: "center", gap: 6, marginBottom: 4 }}>
          <span style={{ width: 10, height: 10, borderRadius: "50%", background: "#3b82f6", flexShrink: 0 }} />
          <span style={{ fontSize: 12, color: "#374151" }}>
            {centerPoint ? `${centerPoint.lat.toFixed(5)}, ${centerPoint.lon.toFixed(5)}` : "Merkez nokta"}
          </span>
        </div>

        <div style={{ display: "flex", gap: 6 }}>
          <button
            onClick={() => { onPickModeChange(pickMode === "center" ? null : "center"); onSearchTargetChange(null); }}
            style={{
              flex: 1, padding: "6px 0", borderRadius: 6,
              border: pickMode === "center" ? "2px solid #3b82f6" : "1px solid #e2e8f0",
              background: pickMode === "center" ? "rgba(59,130,246,.05)" : "#ffffff",
              fontSize: 12, color: "#0f172a", cursor: "pointer",
            }}
          >
            Haritada Seç
          </button>
          <button
            onClick={() => { onSearchTargetChange(searchTarget === "center" ? null : "center"); onPickModeChange(null); }}
            style={{
              flex: 1, padding: "6px 0", borderRadius: 6,
              border: searchTarget === "center" ? "2px solid #22c55e" : "1px solid #e2e8f0",
              background: searchTarget === "center" ? "rgba(34,197,94,.05)" : "#ffffff",
              fontSize: 12, color: "#0f172a", cursor: "pointer",
            }}
          >
            Konum Ara
          </button>
        </div>

        {searchTarget && (
          <p style={{ fontSize: 11, color: "#16a34a", margin: 0 }}>
            Sol alttaki arama kutusunu kullanın.
          </p>
        )}

        <div style={{ marginTop: 4 }}>
          <p style={{ fontSize: 12, fontWeight: 600, color: "#0f172a", margin: "0 0 6px" }}>
            Süreler (dakika) — {thresholds.length}/{MAX_THRESHOLDS}
          </p>

          <div style={{ display: "flex", flexWrap: "wrap", gap: 6, marginBottom: 8 }}>
            {thresholds.map((t) => (
              <span
                key={t}
                style={{
                  display: "flex", alignItems: "center", gap: 4,
                  padding: "3px 8px", borderRadius: 20,
                  background: "#f1f5f9", fontSize: 12, color: "#374151",
                }}
              >
                {t} dk
                <button
                  onClick={() => handleRemoveThreshold(t)}
                  style={{
                    border: "none", background: "transparent", color: "#94a3b8",
                    fontSize: 12, cursor: "pointer", padding: 0, lineHeight: 1,
                  }}
                >
                  ✕
                </button>
              </span>
            ))}
            {thresholds.length === 0 && (
              <span style={{ fontSize: 12, color: "#94a3b8" }}>Henüz süre eklenmedi.</span>
            )}
          </div>

          <div style={{ display: "flex", gap: 6 }}>
            <input
              type="number"
              min={MIN_MINUTES}
              max={MAX_MINUTES}
              placeholder="Örn. 20"
              value={newThreshold}
              onChange={(e) => setNewThreshold(e.target.value)}
              onKeyDown={(e) => { if (e.key === "Enter") handleAddThreshold(); }}
              disabled={thresholds.length >= MAX_THRESHOLDS}
              style={{
                flex: 1, padding: "6px 10px", borderRadius: 6,
                border: "1px solid #e2e8f0", fontSize: 12, color: "#0f172a", outline: "none",
              }}
            />
            <button
              onClick={handleAddThreshold}
              disabled={!newThreshold.trim() || thresholds.length >= MAX_THRESHOLDS}
              style={{
                padding: "6px 12px", borderRadius: 6, border: "none",
                background: !newThreshold.trim() || thresholds.length >= MAX_THRESHOLDS ? "#94a3b8" : "#0f172a",
                color: "#ffffff", fontSize: 12, fontWeight: 500,
                cursor: !newThreshold.trim() || thresholds.length >= MAX_THRESHOLDS ? "not-allowed" : "pointer",
              }}
            >
              Ekle
            </button>
          </div>
        </div>

        <button
          onClick={handleCompute}
          disabled={!centerPoint || (thresholds.length === 0 && !newThreshold.trim()) || isLoading}
          style={{
            marginTop: 4, padding: "9px 0", borderRadius: 8, border: "none",
            background: !centerPoint || thresholds.length === 0 || isLoading ? "#94a3b8" : "#0f172a",
            color: "#ffffff", fontSize: 13, fontWeight: 600,
            cursor: !centerPoint || thresholds.length === 0 || isLoading ? "not-allowed" : "pointer",
          }}
        >
          {isLoading ? "Hesaplanıyor..." : "Hizmet Alanını Hesapla"}
        </button>
        {hasResult && (
          <button
            onClick={() => { onResult(null); setHasResult(false); setError(null); }}
            style={{
              padding: "8px 0", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
            }}
          >
            Temizle
          </button>
        )}
      </div>

      <div style={{ flex: 1, overflowY: "auto", padding: 16 }}>
        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 12px" }}>{error}</p>}

        {hasResult && !error && (
          <>
            <p style={{ fontSize: 12, fontWeight: 600, color: "#0f172a", margin: "0 0 10px" }}>Lejant</p>
            <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
              {[...thresholds].sort((a, b) => a - b).map((minutes, i) => {
                const opacity = 0.5 - (i / Math.max(thresholds.length - 1, 1)) * 0.4;
                return (
                  <div key={minutes} style={{ display: "flex", alignItems: "center", gap: 8 }}>
                    <span style={{
                      width: 20, height: 14, borderRadius: 3,
                      background: `rgba(3, 12, 33, ${opacity})`,
                      border: "1px solid #030c21",
                    }} />
                    <span style={{ fontSize: 12, color: "#374151" }}>0–{minutes} dakika</span>
                  </div>
                );
              })}
            </div>
          </>
        )}

        {!hasResult && !error && (
          <p style={{ fontSize: 13, color: "#94a3b8", textAlign: "center", padding: "20px 0" }}>
            Bir merkez nokta ve süre seçip hesaplayın.
          </p>
        )}
      </div>
    </div>
  );
}