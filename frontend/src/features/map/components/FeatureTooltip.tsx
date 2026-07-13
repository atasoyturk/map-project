import type { TooltipInfo } from "../hooks/useFeatureTooltip";

interface FeatureTooltipProps {
  info: TooltipInfo | null;
}

export function FeatureTooltip({ info }: FeatureTooltipProps) {
  if (!info) return null;

  return (
    <div
      className="pointer-events-none absolute z-[900] -translate-x-1/2 -translate-y-full rounded-lg bg-slate-900/95 px-3 py-2 text-xs text-slate-100 shadow-lg ring-1 ring-white/10"
      style={{ left: info.x, top: info.y - 12 }}
    >
      <div className="font-medium text-slate-50">{info.email}</div>
      {info.teamName && <div className="text-slate-400">{info.teamName}</div>}
      {info.createdDate && <div className="mt-1 text-[10px] text-slate-500">{info.createdDate}</div>}
    </div>
  );
}