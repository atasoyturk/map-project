interface ConfirmModalProps {
  message:   string;
  onConfirm: () => void;
  onCancel:  () => void;
}

export function ConfirmModal({ message, onConfirm, onCancel }: ConfirmModalProps) {
  return (
    <div
      onClick={onCancel}
      style={{
        position: "fixed", inset: 0, zIndex: 3000,
        background: "rgba(0,0,0,0.5)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 14,
          padding: "24px 28px", width: 300,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
        }}
      >
        <h2 style={{ fontSize: 16, fontWeight: 600, color: "#0f172a", margin: "0 0 8px" }}>
          Emin misiniz?
        </h2>
        <p style={{ fontSize: 13, color: "#64748b", margin: "0 0 20px" }}>
          {message}
        </p>
        <div style={{ display: "flex", gap: 10 }}>
          <button
            onClick={onCancel}
            style={{
              flex: 1, padding: "8px 0", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={onConfirm}
            style={{
              flex: 1, padding: "8px 0", borderRadius: 8, border: "none",
              background: "#ef4444", color: "#ffffff",
              fontSize: 13, fontWeight: 600, cursor: "pointer",
            }}
          >
            Sil
          </button>
        </div>
      </div>
    </div>
  );
}