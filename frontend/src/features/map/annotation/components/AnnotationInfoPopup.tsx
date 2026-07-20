import Feature from "ol/Feature";
import { useState } from "react"; 


interface AnnotationInfoPopupProps {
  feature: Feature;
  userLookup: any;
  onClose: () => void;
  onDelete: () => void; 
  canDelete: boolean;   
}

export function AnnotationInfoPopup({feature, userLookup, onClose, onDelete, canDelete}: AnnotationInfoPopupProps) {
  const [showConfirm, setShowConfirm] = useState(false); // Onay state'i
  const noteText = feature.get("noteText") || "Not içeriği bulunamadı.";
  const userId   = feature.get("userId");
  const date     = feature.get("createdDate");
  
  const userName = userId ? (userLookup.get(userId)?.fullName || "Bilinmeyen Kullanıcı") : "Bilinmeyen Kullanıcı";

  const formattedDate = date ? new Intl.DateTimeFormat('tr-TR', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date)).replace(/\//g, '.') : "";

    return (
    <div style={{
      position: "absolute",
      bottom: "100%", left: "50%", transform: "translateX(-50%) translateY(-10px)",
      width: 240, background: "white", borderRadius: 12, padding: 16,
      boxShadow: "0 10px 25px rgba(0,0,0,0.15)", zIndex: 1000,
      border: "1px solid #e2e8f0"
    }}>
      <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 8 }}>
        <span style={{ fontSize: 11, fontWeight: 600, color: "#64748b", textTransform: "uppercase" }}>Saha Notu</span>
        <button onClick={onClose} style={{ border: "none", background: "none", cursor: "pointer", color: "#94a3b8", fontSize: 16 }}>×</button>
      </div>

      <p style={{ margin: "0 0 12px", fontSize: 14, color: "#1e293b", lineHeight: "1.5", whiteSpace: "pre-wrap" }}>
        {noteText}
      </p>
      
      <div style={{ borderTop: "1px solid #f1f5f9", paddingTop: 8, marginTop: 8, display: "flex", justifyContent: "space-between", alignItems: "flex-end" }}>
        <div>
          <div style={{ fontSize: 12, fontWeight: 500, color: "#0f172a" }}>{userName}</div>
          <div style={{ fontSize: 11, color: "#94a3b8" }}>{formattedDate}</div>
        </div>

        {canDelete && (
          <div style={{ position: "relative" }}>
            {showConfirm ? (
              <div style={{ display: "flex", gap: 4 }}>
                <button 
                  onClick={onDelete}
                  style={{ border: "none", background: "#ef4444", color: "white", fontSize: 10, cursor: "pointer", fontWeight: 600, padding: "4px 8px", borderRadius: "4px" }}
                >
                  Evet
                </button>
                <button 
                  onClick={() => setShowConfirm(false)}
                  style={{ border: "1px solid #e2e8f0", background: "white", color: "#64748b", fontSize: 10, cursor: "pointer", fontWeight: 600, padding: "4px 8px", borderRadius: "4px" }}
                >
                  Hayır
                </button>
              </div>
            ) : (
              <button 
                onClick={() => setShowConfirm(true)}
                style={{ border: "none", background: "#fef2f2", color: "#ef4444", fontSize: 11, cursor: "pointer", fontWeight: 600, padding: "4px 8px", borderRadius: "6px" }}
              >
                Sil
              </button>
            )}
          </div>
        )}
      </div>

      <div style={{
        position: "absolute", top: "100%", left: "50%", transform: "translateX(-50%)",
        borderWidth: "8px", borderStyle: "solid", borderColor: "white transparent transparent transparent"
      }} />
    </div>
  );
}
