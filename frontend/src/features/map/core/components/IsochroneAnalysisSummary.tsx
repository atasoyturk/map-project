import type { IsochroneAnalysisBandDto } from "../api/thematicMapService";
import { buildBandSummaries } from "./isochroneSummary";

export function IsochroneAnalysisSummary({ bands }: { bands: IsochroneAnalysisBandDto[] }) {
  return (
    <div className="mt-4 flex flex-col gap-3">
      <p className="m-0 text-xs font-semibold text-slate-900">Erişilebilen yerler</p>
      {buildBandSummaries(bands).map((band) => (
        <div key={band.minutes} className="rounded-lg border border-slate-200 p-2.5">
          <p className="m-0 mb-1.5 text-xs font-semibold text-slate-700">{band.title}</p>
          <ul className="m-0 flex list-none flex-col gap-1 p-0">
            {band.rows.map((row) => (
              <li key={row.key} className="flex justify-between text-xs text-gray-700">
                <span>{row.label}</span>
                <span className="font-medium">{row.count}</span>
              </li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  );
}
