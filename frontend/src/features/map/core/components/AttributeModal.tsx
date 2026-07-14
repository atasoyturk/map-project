import { useState } from "react";
import type { PendingGeometry } from "../types";


interface AttributeModalProps {
  pending:   PendingGeometry;
  onSave:    (name: string, color: string) => void;
  onCancel:  () => void;
  isSaving:  boolean;
}

const TYPE_LABEL: Record<string, string> = {
  Point:      "Nokta",
  LineString: "Çizgi",
  Polygon:    "Poligon",
};

export function AttributeModal({
  pending,
  onSave,
  onCancel,
  isSaving,
}: AttributeModalProps) {
  const [name,  setName]  = useState("");
  const [color, setColor] = useState("#030c21");

  function handleSave() {
    onSave(name, color);
  }

  return (
    // Backdrop
    <div
      style={{
        position:        "fixed",
        inset:           0,
        zIndex:          2000,
        background:      "rgba(0,0,0,0.5)",
        display:         "flex",
        alignItems:      "center",
        justifyContent:  "center",
      }}
      onClick={onCancel}  
    >
      {/* Modal box */}
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background:   "#ffffff",
          borderRadius: 16,
          padding:      "28px 32px",
          width:        360,
          boxShadow:    "0 20px 60px rgba(0,0,0,0.3)",
        }}
      >
        {/* Header */}
        <div style={{ marginBottom: 20 }}>
          <h2
            style={{
              fontSize:   18,
              fontWeight: 600,
              color:      "#0f172a",
              margin:     "4px 0 0",
            }}
          >
            {TYPE_LABEL[pending.type]} Türü
          </h2>
        </div>

        {/* Name */}
        <div style={{ marginBottom: 16 }}>
          <label
            style={{
              display:    "block",
              fontSize:   12,
              fontWeight: 500,
              color:      "#374151",
              marginBottom: 6,
            }}
          >
            İsim
          </label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Örn: Ankara Merkez"
            style={{
              width:        "100%",
              boxSizing:    "border-box",
              padding:      "8px 12px",
              borderRadius: 8,
              border:       "1px solid #e2e8f0",
              fontSize:     14,
              color:        "#0f172a",
              outline:      "none",
            }}
            onFocus={(e) => (e.target.style.borderColor = "#6366f1")}
            onBlur={(e)  => (e.target.style.borderColor = "#e2e8f0")}
          />
        </div>

        {/* Color */}
        <div style={{ marginBottom: 24 }}>
          <label
            style={{
              display:      "block",
              fontSize:     12,
              fontWeight:   500,
              color:        "#374151",
              marginBottom: 6,
            }}
          >
            Renk
          </label>
          <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
            <input
              type="color"
              value={color}
              onChange={(e) => setColor(e.target.value)}
              style={{
                width:        44,
                height:       36,
                borderRadius: 8,
                border:       "1px solid #e2e8f0",
                cursor:       "pointer",
                padding:      2,
              }}
            />
            <span style={{ fontSize: 13, color: "#64748b", fontFamily: "monospace" }}>
              {color.toUpperCase()}
            </span>
          </div>
        </div>

        {/* Buttons */}
        <div style={{ display: "flex", gap: 10 }}>
          <button
            onClick={onCancel}
            disabled={isSaving}
            style={{
              flex:         1,
              padding:      "9px 0",
              borderRadius: 8,
              border:       "1px solid #e2e8f0",
              background:   "#f8fafc",
              color:        "#64748b",
              fontSize:     13,
              fontWeight:   500,
              cursor:       "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={handleSave}
            disabled={isSaving}
            style={{
              flex:         2,
              padding:      "9px 0",
              borderRadius: 8,
              border:       "1px solid rgba(255,255,255,.15)",
              background:   isSaving ? "#1e293b" : "#030c21",
              color:        isSaving ? "#64748b" : "#94a3b8",
              fontSize:     13,
              fontWeight:   600,
              cursor:       isSaving ? "not-allowed" : "pointer",
            }}
          >
            {isSaving ? "Kaydediliyor..." : "Kaydet"}
          </button>
        </div>
      </div>
    </div>
  );
}