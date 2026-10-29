import { useMemo, useState, type ReactNode } from "react";
import { useAuth } from "../../../auth/context/AuthContext";
import { getLocationReport, type LocationReportDto, type NearbyPlaceDto } from "../api/locationReportService";
import type { IsochroneResultDto } from "../api/thematicMapService";
import { flattenCategories, type CategoryTreeNode } from "../../../../shared/utils/categoryTree";
import { Z } from "../../../../shared/components/ui/zIndex";
import {
  MAX_COMPLEMENTS, formatArea, formatMeters, formatSaturationRatio, formatWeight, parseCategorySelection,
  type ReportCategorySelection,
} from "./locationReport";

type PickMode = "center" | null;

const SELECTION_STORAGE_KEY = "locationReport.categories";

function loadStoredSelection(): string | null {
  try { return localStorage.getItem(SELECTION_STORAGE_KEY); } catch { return null; }
}

function storeSelection(selection: ReportCategorySelection) {
  try { localStorage.setItem(SELECTION_STORAGE_KEY, JSON.stringify(selection)); } catch { /* storage unavailable */ }
}

interface LocationReportPanelProps {
  onClose:              () => void;
  categories:           CategoryTreeNode[];
  pickMode:             PickMode;
  onPickModeChange:     (mode: PickMode) => void;
  searchTarget:         PickMode;
  onSearchTargetChange: (target: PickMode) => void;
  center:               { lon: number; lat: number; address?: string } | null;
  onResult:             (result: IsochroneResultDto | null) => void;
}

export function LocationReportPanel({
  onClose, categories, pickMode, onPickModeChange, searchTarget, onSearchTargetChange, center, onResult,
}: LocationReportPanelProps) {
  const { apiFetch } = useAuth();

  const flatCategories = useMemo(() => flattenCategories(categories), [categories]);
  const categoryNames  = useMemo(() => new Map(flatCategories.map((c) => [c.id, c.name])), [flatCategories]);

  // Until the user changes it, the selection comes from the last report (categories load asynchronously).
  const [editedSelection, setEditedSelection] = useState<ReportCategorySelection | null>(null);
  const storedSelection = useMemo(
    () => parseCategorySelection(loadStoredSelection(), new Set(categoryNames.keys())),
    [categoryNames],
  );
  const selection = editedSelection ?? storedSelection ?? { competitorId: 0, complementIds: [] };

  const [report,      setReport]      = useState<LocationReportDto | null>(null);
  const [reportMeta,  setReportMeta]  = useState({ title: "", date: "", competitorName: "" });
  const [isLoading,   setIsLoading]   = useState(false);
  const [error,       setError]       = useState<string | null>(null);

  const canCompute = !!center && selection.competitorId !== 0 && selection.complementIds.length > 0 && !isLoading;

  function setCompetitor(id: number) {
    setEditedSelection({ competitorId: id, complementIds: selection.complementIds.filter((c) => c !== id) });
  }

  function addComplement(id: number) {
    if (!id || id === selection.competitorId || selection.complementIds.includes(id)) return;
    setEditedSelection({ ...selection, complementIds: [...selection.complementIds, id] });
  }

  function removeComplement(id: number) {
    setEditedSelection({ ...selection, complementIds: selection.complementIds.filter((c) => c !== id) });
  }

  async function handleCompute() {
    if (!center || !canCompute) return;

    setIsLoading(true);
    setError(null);
    storeSelection(selection);

    try {
      const res = await getLocationReport(apiFetch, center.lon, center.lat, selection.competitorId, selection.complementIds);
      if (!res.ok) {
        const message = await res.text();
        setError(res.status < 500 && message ? message : "Rapor oluşturulamadı.");
        setReport(null);
        onResult(null);
        return;
      }

      const data: LocationReportDto = await res.json();
      setReport(data);
      setReportMeta({
        title:          center.address ?? `${center.lat.toFixed(5)}, ${center.lon.toFixed(5)}`,
        date:           new Date().toLocaleString("tr-TR"),
        competitorName: categoryNames.get(selection.competitorId) ?? "",
      });
      // Largest band first, so the map layer draws inner bands on top.
      onResult({ bands: [...data.bands].reverse() });
    } catch {
      setError("Sunucuya bağlanılamadı.");
      setReport(null);
      onResult(null);
    } finally {
      setIsLoading(false);
    }
  }

  function handleClear() {
    setReport(null);
    setError(null);
    onResult(null);
  }

  return (
    <div
      className="location-report-print fixed top-[70px] bottom-[100px] left-4 flex w-[360px] flex-col overflow-hidden rounded-xl border border-slate-200 bg-white shadow-xl print:w-full print:overflow-visible print:rounded-none print:border-0 print:shadow-none"
      style={{ zIndex: Z.sidePanel }}
    >
      <div className="flex items-center justify-between border-b border-slate-100 px-4 py-3">
        <span className="text-sm font-semibold text-slate-900">Konum Raporu</span>
        <div className="flex items-center gap-2 print:hidden">
          {report && (
            <button onClick={() => window.print()} className="cursor-pointer rounded-md border border-slate-200 bg-white px-2 py-1 text-xs text-slate-700">
              Yazdır / PDF
            </button>
          )}
          <button onClick={onClose} className="cursor-pointer border-0 bg-transparent text-base text-slate-400">✕</button>
        </div>
      </div>

      <div className="flex flex-col gap-2 border-b border-slate-100 p-4 print:hidden">
        <div className="flex items-center gap-1.5 text-xs text-gray-700">
          <span className="h-2.5 w-2.5 shrink-0 rounded-full bg-blue-500" />
          {center ? (center.address ?? `${center.lat.toFixed(5)}, ${center.lon.toFixed(5)}`) : "Aday konum"}
        </div>

        <div className="flex gap-1.5">
          <button
            onClick={() => { onPickModeChange(pickMode === "center" ? null : "center"); onSearchTargetChange(null); }}
            className={`flex-1 cursor-pointer rounded-md py-1.5 text-xs text-slate-900 ${pickMode === "center" ? "border-2 border-blue-500 bg-blue-500/5" : "border border-slate-200 bg-white"}`}
          >
            Haritada Seç
          </button>
          <button
            onClick={() => { onSearchTargetChange(searchTarget === "center" ? null : "center"); onPickModeChange(null); }}
            className={`flex-1 cursor-pointer rounded-md py-1.5 text-xs text-slate-900 ${searchTarget === "center" ? "border-2 border-green-500 bg-green-500/5" : "border border-slate-200 bg-white"}`}
          >
            Adres Ara
          </button>
        </div>
        {searchTarget && <p className="m-0 text-[11px] text-green-600">Sol alttaki arama kutusunu kullanın.</p>}

        <label className="mt-1 text-xs font-semibold text-slate-900">
          Rakip kategori
          <select
            value={selection.competitorId}
            onChange={(e) => setCompetitor(Number(e.target.value))}
            className="mt-1 w-full rounded-md border border-slate-200 px-2 py-1.5 text-xs font-normal"
          >
            <option value={0} disabled>Seçin...</option>
            {flatCategories.map((c) => (
              <option key={c.id} value={c.id}>{"— ".repeat(c.depth)}{c.name}</option>
            ))}
          </select>
        </label>

        <div className="text-xs font-semibold text-slate-900">
          Tamamlayıcı kategoriler — {selection.complementIds.length}/{MAX_COMPLEMENTS}
          <div className="mt-1 flex flex-wrap gap-1.5">
            {selection.complementIds.map((id) => (
              <span key={id} className="flex items-center gap-1 rounded-full bg-slate-100 px-2 py-0.5 text-xs font-normal text-gray-700">
                {categoryNames.get(id)}
                <button onClick={() => removeComplement(id)} className="cursor-pointer border-0 bg-transparent p-0 text-xs text-slate-400">✕</button>
              </span>
            ))}
          </div>
          {selection.complementIds.length < MAX_COMPLEMENTS && (
            <select
              value={0}
              onChange={(e) => addComplement(Number(e.target.value))}
              aria-label="Tamamlayıcı kategori ekle"
              className="mt-1 w-full rounded-md border border-slate-200 px-2 py-1.5 text-xs font-normal"
            >
              <option value={0} disabled>Tamamlayıcı ekle...</option>
              {flatCategories
                .filter((c) => c.id !== selection.competitorId && !selection.complementIds.includes(c.id))
                .map((c) => (
                  <option key={c.id} value={c.id}>{"— ".repeat(c.depth)}{c.name}</option>
                ))}
            </select>
          )}
        </div>

        <button
          onClick={handleCompute}
          disabled={!canCompute}
          className={`mt-1 rounded-lg border-0 py-2 text-[13px] font-semibold text-white ${canCompute ? "cursor-pointer bg-slate-900" : "cursor-not-allowed bg-slate-400"}`}
        >
          {isLoading ? "Hazırlanıyor..." : "Rapor Oluştur"}
        </button>
        {report && (
          <button onClick={handleClear} className="cursor-pointer rounded-lg border border-slate-200 bg-slate-50 py-2 text-[13px] font-medium text-slate-500">
            Temizle
          </button>
        )}
      </div>

      <div className="flex-1 overflow-y-auto p-4 print:overflow-visible">
        {error && <p className="m-0 mb-3 text-xs text-red-600">{error}</p>}

        {!report && !error && (
          <p className="py-5 text-center text-[13px] text-slate-400">
            Aday konumu ve kategorileri seçip raporu oluşturun.
          </p>
        )}

        {report && (
          <div className="flex flex-col gap-4 text-xs text-gray-700">
            <div>
              <p className="m-0 text-[13px] font-semibold text-slate-900">{reportMeta.title}</p>
              <p className="m-0 text-[11px] text-slate-500">{reportMeta.date} · Araçla 5/10/15 dk</p>
            </div>

            <section className="rounded-lg border border-slate-200 p-3">
              <div className="flex items-baseline gap-1">
                <span className="text-3xl font-bold text-slate-900">{report.score.total}</span>
                <span className="text-slate-500">/ 100</span>
              </div>
              <p className="m-0 mb-2 text-[11px] text-slate-500">
                Göreceli skor: adayları karşılaştırmak içindir, tek başına bir uygunluk eşiği değildir.
              </p>
              <ScoreRow label="Tamamlayıcı yoğunluğu" weight={report.score.complementWeight} value={report.score.complementScore}
                hint="5 dk içi tam, 5–10 dk yarım, 10–15 dk çeyrek ağırlıkla sayılır." />
              <ScoreRow label="Rekabet" weight={report.score.competitionWeight} value={report.score.competitionScore}
                hint={`10 dk içinde tamamlayıcı başına rakip: ${formatSaturationRatio(report.score.saturationRatio)}. Tamamlayıcı yoksa talep doğrulanamaz ve puan 0 olur.`} />
              <ScoreRow label="Ulaşım" weight={report.score.transitWeight} value={report.score.transitScore}
                hint="En yakın durağa uzaklık ve 10 dk içindeki durak sayısı." />
            </section>

            <section>
              <SectionTitle>Hizmet alanı</SectionTitle>
              <table className="w-full border-collapse text-right">
                <thead className="text-[11px] text-slate-500">
                  <tr>
                    <th className="text-left font-medium">Süre</th>
                    <th className="font-medium">Alan</th>
                    <th className="font-medium">Rakip</th>
                    <th className="font-medium">Tamamlayıcı</th>
                    <th className="font-medium">Durak</th>
                  </tr>
                </thead>
                <tbody>
                  {report.bands.map((b) => (
                    <tr key={b.minutesThreshold} className="border-t border-slate-100">
                      <td className="py-1 text-left">{b.minutesThreshold} dk</td>
                      <td>{formatArea(b.areaSquareMeters)}</td>
                      <td>{b.competitorCount}</td>
                      <td>{b.complementCount}</td>
                      <td>{b.stopCount}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <p className="m-0 mt-1 text-[11px] text-slate-500">Sayılar birikimlidir: 10 dk, 5 dk içindekileri de kapsar.</p>
            </section>

            <section>
              <SectionTitle>Rakipler — {reportMeta.competitorName}</SectionTitle>
              <NearbyList title="En yakın rakipler (kuş uçuşu)" places={report.nearestCompetitors} />
            </section>

            <section>
              <SectionTitle>Tamamlayıcılar</SectionTitle>
              <table className="w-full border-collapse text-right">
                <thead className="text-[11px] text-slate-500">
                  <tr>
                    <th className="text-left font-medium">Kategori</th>
                    {report.bands.map((b) => <th key={b.minutesThreshold} className="font-medium">{b.minutesThreshold} dk</th>)}
                  </tr>
                </thead>
                <tbody>
                  {report.bands[0]?.complementCounts.map((c, i) => (
                    <tr key={c.categoryId} className="border-t border-slate-100">
                      <td className="py-1 text-left">{c.categoryName}</td>
                      {report.bands.map((b) => <td key={b.minutesThreshold}>{b.complementCounts[i]?.count ?? 0}</td>)}
                    </tr>
                  ))}
                </tbody>
              </table>
            </section>

            <section>
              <SectionTitle>Toplu taşıma</SectionTitle>
              <NearbyList title="En yakın duraklar (kuş uçuşu)" places={report.nearestStops} />
            </section>

            <section>
              <SectionTitle>Nüfus</SectionTitle>
              <p className="m-0 text-slate-500">Nüfus verisi henüz eklenmedi.</p>
            </section>

            <footer className="border-t border-slate-100 pt-2 text-[11px] text-slate-500">
              <p className="m-0">© OpenStreetMap contributors</p>
              <p className="m-0">Sayılar OpenStreetMap verisinin eksiksizliğine bağlıdır.</p>
            </footer>
          </div>
        )}
      </div>
    </div>
  );
}

function SectionTitle({ children }: { children: ReactNode }) {
  return <p className="m-0 mb-1.5 text-xs font-semibold text-slate-900">{children}</p>;
}

function ScoreRow({ label, weight, value, hint }: { label: string; weight: number; value: number; hint: string }) {
  return (
    <div className="border-t border-slate-100 py-1.5">
      <div className="flex justify-between">
        <span>{label} <span className="text-slate-400">({formatWeight(weight)})</span></span>
        <span className="font-semibold text-slate-900">{value}</span>
      </div>
      <p className="m-0 text-[11px] text-slate-500">{hint}</p>
    </div>
  );
}

function NearbyList({ title, places }: { title: string; places: NearbyPlaceDto[] }) {
  return (
    <>
      <p className="m-0 mb-1 text-[11px] text-slate-500">{title}</p>
      {places.length === 0 ? (
        <p className="m-0 text-slate-400">Kayıt yok.</p>
      ) : (
        <ul className="m-0 flex list-none flex-col gap-1 p-0">
          {places.map((p, i) => (
            <li key={i} className="flex justify-between">
              <span>{p.name || "(isimsiz)"}</span>
              <span className="font-medium">{formatMeters(p.distanceMeters)}</span>
            </li>
          ))}
        </ul>
      )}
    </>
  );
}
