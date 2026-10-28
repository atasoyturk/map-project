import { useEffect, type ReactNode } from "react";
import { Z } from "./zIndex";

interface ModalProps {
  onClose:    () => void;
  children:   ReactNode;
  className?: string;
}

export function Modal({ onClose, children, className = "" }: ModalProps) {
  useEffect(() => {
    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === "Escape") onClose();
    }
    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [onClose]);

  return (
    <div
      onClick={onClose}
      style={{ zIndex: Z.modalRaised }}
      className="fixed inset-0 flex items-center justify-center bg-black/50"
    >
      <div
        onClick={(e) => e.stopPropagation()}
        className={`bg-white shadow-[0_20px_60px_rgba(0,0,0,0.25)] ${className}`}
      >
        {children}
      </div>
    </div>
  );
}
