import { useState } from "react";

interface AnnotationModalProps {
  onSave:   (noteText: string) => void;
  onCancel: () => void;
  isSaving: boolean;
  error?:   string | null;   // ← YENİ
}

export function AnnotationModal({ onSave, onCancel, isSaving, error }: AnnotationModalProps) {
  const [noteText, setNoteText] = useState("");

  function handleSave() {
    if (!noteText.trim()) return;
    onSave(noteText.trim());
  }

  return (
    <div
      style={{
        position:       "fixed",
        inset:          0,
        zIndex:         2000,
        background:     "rgba(0,0,0,0.5)",
        display:        "flex",
        alignItems:     "center",
        justifyContent: "center",
      }}
      onClick={onCancel}
    >
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
        <div style={{ marginBottom: 20 }}>
          <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "4px 0 0" }}>
            Not Ekle
          </h2>
        </div>

        <div style={{ marginBottom: error ? 8 : 24 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Not Metni
          </label>
          <textarea
            value={noteText}
            onChange={(e) => setNoteText(e.target.value)}
            placeholder="Bu konuma dair notunuz..."
            rows={4}
            style={{
              width:        "100%",
              boxSizing:    "border-box",
              padding:      "8px 12px",
              borderRadius: 8,
              border:       "1px solid #e2e8f0",
              fontSize:     14,
              color:        "#0f172a",
              outline:      "none",
              resize:       "vertical",
              fontFamily:   "inherit",
            }}
            onFocus={(e) => (e.target.style.borderColor = "#6366f1")}
            onBlur={(e)  => (e.target.style.borderColor = "#e2e8f0")}
          />
        </div>

        {error && (
          <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 16px" }}>
            {error}
          </p>
        )}

        <div style={{ display: "flex", gap: 10 }}>
          <button
            onClick={onCancel}
            disabled={isSaving}
            style={{
              flex: 1, padding: "9px 0", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={handleSave}
            disabled={isSaving || !noteText.trim()}
            style={{
              flex: 2, padding: "9px 0", borderRadius: 8,
              border: "1px solid rgba(255,255,255,.15)",
              background: isSaving ? "#1e293b" : "#030c21",
              color:      isSaving ? "#64748b" : "#94a3b8",
              fontSize: 13, fontWeight: 600,
              cursor: isSaving || !noteText.trim() ? "not-allowed" : "pointer",
            }}
          >
            {isSaving ? "Kaydediliyor..." : "Kaydet"}
          </button>
        </div>
      </div>
    </div>
  );
}