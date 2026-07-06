import { useState, type SyntheticEvent } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { GlobeCanvas } from "../components/GlobeCanvas";


export function RegisterPage() {
  const [email, setEmail]       = useState("");
  const [password, setPassword] = useState("");
  const [error, setError]       = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const { apiFetch } = useAuth();
  const navigate     = useNavigate();

  async function handleSubmit(e: SyntheticEvent<HTMLFormElement>) {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      const response = await apiFetch("/api/auth/register", {
        method: "POST",
        body: JSON.stringify({ email, password }),
      });

      if (response.status === 409) {
        setError("Bu e-posta adresi zaten kayıtlı.");
        return;
      }

      if (!response.ok) {
        setError("Kayıt sırasında bir hata oluştu.");
        return;
      }

      navigate("/login");
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div
      className="min-h-screen w-full flex items-center justify-center"
      style={{ background: "#030d1a" }}
    >
      <div className="flex flex-col md:flex-row items-center justify-center gap-16 px-8">

        {/* Left — Globe */}
        <div className="flex flex-col items-center gap-6">
          <GlobeCanvas />
          <div className="text-center">
            <p style={{ color: "rgba(186,230,253,.85)", fontSize: 14, fontWeight: 500, letterSpacing: ".5px" }}>
              Coğrafi Bilgi Sistemi
            </p>
            <p style={{ color: "rgba(148,163,184,.5)", fontSize: 11, marginTop: 4 }}>
              Güvenli Erişim Platformu
            </p>
          </div>
        </div>

        {/* Right — Form */}
        <div
          className="w-full max-w-sm rounded-2xl p-8"
          style={{ background: "#ffffff", minWidth: 320, color: "#0f172a" }}
        >
          <div className="flex items-center gap-2 mb-8">
            <div className="w-2 h-2 rounded-full" style={{ background: "#3b82f6" }} />
            <span style={{ fontWeight: 600, color: "#0f172a", fontSize: 14, letterSpacing: ".3px" }}>GisPortal</span>
          </div>

          <h1 style={{ fontSize: 24, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>Kayıt Ol</h1>
          <p style={{ fontSize: 14, color: "#64748b", marginBottom: 24 }}>
            Hesabınızı oluşturmak için bilgilerinizi girin.
          </p>

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label htmlFor="email" style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 4 }}>
                E-posta
              </label>
              <input id="email" type="email" required value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                style={{ borderColor: "#cbd5e1", color: "#0f172a", background: "#f8fafc" }}
                placeholder="ornek@sirket.com" />
            </div>
            <div>
              <label htmlFor="password" style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 4 }}>
                Şifre
              </label>
              <input id="password" type="password" required minLength={6}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                style={{ borderColor: "#cbd5e1", color: "#0f172a", background: "#f8fafc" }}
                placeholder="En az 6 karakter" />
            </div>
            {error && (
              <p style={{ fontSize: 14, color: "#dc2626", background: "#fef2f2", border: "1px solid #fecaca", borderRadius: 8, padding: "8px 12px" }}>
                {error}
              </p>
            )}
            <button type="submit" disabled={isLoading}
              className="w-full py-2 rounded-lg text-sm font-medium text-white disabled:opacity-50 disabled:cursor-not-allowed"
              style={{ background: "linear-gradient(to right, #030d1a, #002d8f, #030d1a)", marginTop: 4 }}>
              {isLoading ? "Kayıt olunuyor..." : "Kayıt Ol"}
            </button>
          </form>

          <p style={{ fontSize: 13, color: "#64748b", textAlign: "center", marginTop: 24 }}>
            Zaten hesabınız var mı?{" "}
            <Link to="/login" style={{ color: "#2563eb", fontWeight: 500 }}>Giriş yapın</Link>
          </p>
        </div>

      </div>
    </div>
  );
}