interface HeatmapLegendProps {
  visible: boolean;
}

export function HeatmapLegend({ visible }: HeatmapLegendProps) {
  if (!visible) return null;

  return (
    <div style={{
      position:     "absolute",
      bottom:       24,
      right:        16,
      zIndex:       10,
      background:   "rgba(15,23,42,0.85)",
      borderRadius: 12,
      padding:      "14px 16px",
      boxShadow:    "0 4px 24px rgba(0,0,0,0.3)",
      minWidth:     140,
    }}>
      <span style={{
        fontSize:      11,
        fontWeight:    600,
        color:         "#94a3b8",
        letterSpacing: ".5px",
        display:       "block",
        marginBottom:  10,
      }}>
        Isı haritası
      </span>

      {/* Color gradient */}
      <div style={{
        width:        "100%",
        height:       12,
        borderRadius: 6,
        background:   "linear-gradient(to right, #0000FF, #00FFFF, #00FF00, #FFFF00, #FF0000)",
        marginBottom: 6,
      }} />

      {/* Labels */}
      <div style={{
        display:        "flex",
        justifyContent: "space-between",
      }}>
        <span style={{ fontSize: 10, color: "#64748b" }}>Düşük</span>
        <span style={{ fontSize: 10, color: "#64748b" }}>Orta</span>
        <span style={{ fontSize: 10, color: "#64748b" }}>Yüksek</span>
      </div>

      <div style={{
        display:        "flex",
        justifyContent: "space-between",
        marginTop:      2,
      }}>
        <span style={{ fontSize: 10, color: "#475569" }}>0</span>
        <span style={{ fontSize: 10, color: "#475569" }}>0.5</span>
        <span style={{ fontSize: 10, color: "#475569" }}>1</span>
      </div>
    </div>
  );
}