import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { getRoles, assignRoleToUser, removeRoleFromUser } from "../api/roleService";
import { Modal }  from "../../../shared/components/ui/Modal";
import { Button } from "../../../shared/components/ui/Button";
import type { RoleDto } from "../types";

interface RoleAssignModalProps {
  userId:       number;
  currentRoles: string[];
  onClose:      () => void;
  onUpdated:    () => void;
}

export function RoleAssignModal({ userId, currentRoles, onClose, onUpdated }: RoleAssignModalProps) {
  const [allRoles,  setAllRoles]  = useState<RoleDto[]>([]);
  const [userRoles, setUserRoles] = useState<string[]>(currentRoles);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving,  setIsSaving]  = useState(false);
  const [error,     setError]     = useState<string | null>(null);

  const { apiFetch} = useAuth();

  useEffect(() => {
    async function fetchRoles() {
      setIsLoading(true);
      try {
        const res = await getRoles(apiFetch);
        if (!res.ok) { setError("Roller yüklenemedi."); return; }
        setAllRoles(await res.json());
      } catch {
        setError("Sunucuya bağlanılamadı.");
      }
      finally { setIsLoading(false); }
    }
    fetchRoles();
  }, []);

  async function handleToggle(role: RoleDto) {
    const hasRole = userRoles.includes(role.name);
    setIsSaving(true);
    setError(null);
    try {
      const res = hasRole
        ? await removeRoleFromUser(apiFetch, userId, role.id)
        : await assignRoleToUser(apiFetch, userId, role.id);
      if (!res.ok) { setError("Rol güncellenemedi."); return; }
      setUserRoles(prev => hasRole ? prev.filter(r => r !== role.name) : [...prev, role.name]);
      onUpdated();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    }
    finally { setIsSaving(false); }
  }

  return (
    <Modal onClose={onClose} className="max-h-[80vh] w-[400px] overflow-y-auto rounded-2xl px-8 py-7">
      <h2 className="mt-0 mb-1.5 text-lg font-semibold text-ink">Rol Yönetimi</h2>
      <p className="mt-0 mb-5 text-[13px] text-muted">Kullanıcının rollerini düzenleyin.</p>

      {isLoading ? (
        <p className="text-[13px] text-slate-400">Yükleniyor...</p>
      ) : (
        <div className="flex flex-col gap-2.5">
          {allRoles
            .filter(role => !role.hasAdminAccess)
            .map((role) => {
            const hasRole = userRoles.includes(role.name);
            return (
              <div
                key={role.id}
                className={`flex items-center gap-3 rounded-[10px] border border-line px-3.5 py-3 ${hasRole ? "bg-blue-50" : "bg-white"}`}
              >
                <input
                  type="checkbox"
                  checked={hasRole}
                  disabled={isSaving}
                  onChange={() => handleToggle(role)}
                  className="cursor-pointer disabled:cursor-not-allowed"
                />
                <span className="text-[13px] font-medium text-ink">{role.name}</span>
                {hasRole && (
                  <span className="ml-auto rounded-full bg-blue-50 px-2 py-px text-[10px] font-semibold text-blue-500">
                    Atanmış
                  </span>
                )}
              </div>
            );
          })}
        </div>
      )}

      {error && <p className="mt-4 mb-0 text-xs text-red-600">{error}</p>}

      <Button className="mt-5 w-full" onClick={onClose}>Kapat</Button>
    </Modal>
  );
}
