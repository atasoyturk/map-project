import { useEffect, useRef, useState } from "react";
import { Z } from "../../../../shared/components/ui/zIndex";

interface NavDropdownProps {
  label:    string;
  disabled?: boolean;
  hasActiveItem?: boolean;
  onOpenChange?: (open: boolean) => void;
  children: React.ReactNode;
}

export function NavDropdown({ label, disabled, hasActiveItem, onOpenChange, children }: NavDropdownProps) {
  const [open, setOpenState] = useState(false);

  function setOpen(value: boolean) {
    setOpenState(value);
    onOpenChange?.(value);
  }

  const containerRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <div ref={containerRef} style={{ position: "relative" }}>
      <button
        onClick={() => setOpen(!open)}
        disabled={disabled}
        style={{
          padding: "6px 14px", borderRadius: 8, border: "1px solid",
          borderColor: open || hasActiveItem ? "#3b82f6" : "rgba(255,255,255,.15)",
          background:  open || hasActiveItem ? "rgba(59,130,246,.2)" : "transparent",
          color:       open || hasActiveItem ? "#93c5fd" : "#94a3b8",
          fontSize: 13, fontWeight: 500,
          cursor:   disabled ? "not-allowed" : "pointer",
          opacity:  disabled ? 0.5 : 1,
          transition: "all .15s",
          display: "flex", alignItems: "center", gap: 4,
        }}
      >
        {label}
        <span style={{ fontSize: 10, opacity: 0.7 }}>{open ? "▲" : "▼"}</span>
      </button>

      {open && (
        <div
          onClick={() => setOpen(false)}
          style={{
            position: "absolute", top: "calc(100% + 8px)", left: "50%", transform: "translateX(-50%)",
            display: "flex", flexDirection: "column", gap: 6,
            background: "#030c21", border: "1px solid rgba(255,255,255,.08)",
            borderRadius: 12, padding: 10, minWidth: 180,
            boxShadow: "0 8px 24px rgba(0,0,0,0.3)", zIndex: Z.dropdown,
          }}
        >
          {children}
        </div>
      )}
    </div>
  );
}