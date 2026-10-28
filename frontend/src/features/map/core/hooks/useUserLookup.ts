import { useEffect, useState } from "react";
import { getUserLookup } from "../../../../shared/api/userLookupService";
import type { ApiFetch } from "../../../../shared/api/apiFetch";

export interface UserLookupEntry {
  email:    string;
  teamName: string | null;
}

export function useUserLookup(apiFetch: ApiFetch): Map<number, UserLookupEntry> {
  const [lookup, setLookup] = useState<Map<number, UserLookupEntry>>(new Map());

  useEffect(() => {
    let cancelled = false;

    async function load() {
      try {
        const res = await getUserLookup(apiFetch);
        if (!res.ok) return;

        const data: { id: number; email: string; teamName: string | null }[] = await res.json();
        if (cancelled) return;

        setLookup(new Map(data.map((u) => [u.id, { email: u.email, teamName: u.teamName }])));
      } catch {  }
    }

    load();
    return () => { cancelled = true; };
  }, [apiFetch]);

  return lookup;
}