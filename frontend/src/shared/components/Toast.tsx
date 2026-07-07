import { useEffect } from "react";

interface ToastProps {
  message:  string;
  type:     "success" | "error";
  onClose:  () => void;
}

export function Toast({ message, type, onClose }: ToastProps) {
  // auto close after 4 seconds
  useEffect(() => {
    const timer = setTimeout(onClose, 4000);
    return () => clearTimeout(timer);  // cleanup 
  }, [onClose]);

  const isSuccess = type === "success";

  return (
    <div
      style={{
        position:     "fixed",
        bottom:       24,
        left:         "50%",
        transform:    "translateX(-50%)",
        zIndex:       3000,
        background:   isSuccess ? "#f0fdf4" : "#fef2f2",
        border:       `1px solid ${isSuccess ? "#bbf7d0" : "#fecaca"}`,
        borderRadius: 12,
        padding:      "12px 20px",
        boxShadow:    "0 8px 24px rgba(0,0,0,0.12)",
        display:      "flex",
        alignItems:   "center",
        gap:          10,
        maxWidth:     420,
        minWidth:     280,
      }}
    >
      <span style={{ fontSize: 18 }}>{isSuccess ? "✓" : "✕"}</span>
      <span
        style={{
          fontSize:   13,
          fontWeight: 500,
          color:      isSuccess ? "#166534" : "#991b1b",
          lineHeight: 1.4,
        }}
      >
        {message}
      </span>
      <button
        onClick={onClose}
        style={{
          marginLeft: "auto",
          background: "none",
          border:     "none",
          cursor:     "pointer",
          color:      isSuccess ? "#166534" : "#991b1b",
          fontSize:   16,
          padding:    "0 4px",
        }}
      >
        ×
      </button>
    </div>
  );
}