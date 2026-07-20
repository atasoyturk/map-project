import { useEdgeVisibility } from "../hooks/useEdgeVisibility";
import type { DrawType } from "../types";
import { useEffect } from "react";


const LABEL_MAP: Record<DrawType, string> = {
  Point:      "Nokta",
  LineString: "Çizgi",
  Polygon:    "Poligon",
};

interface BottomToolbarProps {
  activeType:  DrawType | null;
  onSelect:    (type: DrawType) => void;
  onCancel:    () => void;
  disabled:    boolean;
  keepVisible: boolean;
}

export function BottomToolbar({ activeType, onSelect, onCancel, disabled, keepVisible }: BottomToolbarProps) {
  const visible = useEdgeVisibility("bottom", keepVisible);

  useEffect(() => {
    if (!activeType) return;
    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === "Escape") onCancel();
    }
    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [activeType, onCancel]);

  return (
    <nav style={{
      position:     "fixed",
      bottom:       visible ? 20 : -80,
      left:         "50%",
      transform:    "translateX(-50%)",
      display:      "flex",
      alignItems:   "center",
      gap:          8,
      background:   "#030c21",
      border:       "1px solid rgba(255,255,255,.08)",
      borderRadius: 12,
      padding:      "8px 12px",
      zIndex:       1000,
      transition:   "bottom .25s ease",
    }}>
      {(["Point", "LineString", "Polygon"] as DrawType[]).map((type) => (
        <button
          key={type}
          onClick={() => onSelect(type)}
          disabled={disabled}
          style={{
            padding: "6px 14px", borderRadius: 8, border: "1px solid",
            borderColor: activeType === type ? "#3b82f6" : "rgba(255,255,255,.15)",
            background:  activeType === type ? "rgba(6,18,52,.2)" : "transparent",
            color:       activeType === type ? "#93c5fd" : "#94a3b8",
            fontSize: 13, fontWeight: 500,
            cursor:   disabled ? "not-allowed" : "pointer",
            opacity:  disabled ? 0.5 : 1,
            transition: "all .15s",
          }}
        >
          {LABEL_MAP[type]}
        </button>
      ))}

    </nav>
  );
}