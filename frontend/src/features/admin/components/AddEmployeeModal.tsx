import { useState } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { createEmployee } from "../api/userService";

interface AddEmployeeModalProps {
  onClose:   () => void;
  onCreated: () => void;
}

export function AddEmployeeModal({ onClose, onCreated }: AddEmployeeModalProps) {
  const { apiFetch } = useAuth();

  const [email,    setEmail]    = useState("");
  const [password, setPassword] = useState("");
  const [isSaving, setIsSaving] = useState(false);
  const [error,    setError]    = useState<string | null>(null);

  const canSave = !!email.trim() && password.length >= 6 && !isSaving;

  async function handleSave() {
    if (!canSave) return;
    setIsSaving(true);
    setError(null);
    try {
      const res = await createEmployee(apiFetch, email.trim(), password);
      if (!res.ok) {
        setError(res.status === 409 ? "Bu e-posta adresi zaten kayıtlı." : "Çalışan oluşturulamadı.");
        return;
      }
      onCreated();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, zIndex: 2000,
        background: "rgba(0,0,0,0.5)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "28px 32px", width: 380,
          boxShadow: "0 20px 60px rgba(0,0,0,0.3)",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 20px" }}>
          Yeni Çalışan Ekle
        </h2>

        <div style={{ marginBottom: 16 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            E-posta
          </label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="ornek@sirket.com"
            style={{
              width: "100%", boxSizing: "border-box", padding: "8px 12px",
              borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
              color: "#0f172a", outline: "none",
            }}
          />
        </div>

        <div style={{ marginBottom: 24 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Şifre
          </label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="En az 6 karakter"
            style={{
              width: "100%", boxSizing: "border-box", padding: "8px 12px",
              borderRadius: 8, border: "1px solid #e2e8f0", fontSize: 14,
              color: "#0f172a", outline: "none",
            }}
          />
        </div>

        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 16px" }}>{error}</p>}

        <div style={{ display: "flex", gap: 10 }}>
          <button
            onClick={onClose}
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
            disabled={!canSave}
            style={{
              flex: 2, padding: "9px 0", borderRadius: 8, border: "none",
              background: !canSave ? "#94a3b8" : "#0f172a",
              color: "#ffffff", fontSize: 13, fontWeight: 600,
              cursor: !canSave ? "not-allowed" : "pointer",
            }}
          >
            {isSaving ? "Oluşturuluyor..." : "Oluştur"}
          </button>
        </div>
      </div>
    </div>
  );
}