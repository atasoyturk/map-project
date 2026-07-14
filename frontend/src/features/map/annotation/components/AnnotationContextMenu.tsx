interface AnnotationContextMenuProps {
  x: number;
  y: number;
  onAddNote: () => void;
  onClose:   () => void;
}

export function AnnotationContextMenu({ x, y, onAddNote, onClose }: AnnotationContextMenuProps) {
  return (
    <>
      <div className="fixed inset-0 z-[1900]" onClick={onClose} />
      <div
        className="absolute z-[1901] min-w-[140px] rounded-lg bg-white py-1 shadow-lg ring-1 ring-black/10"
        style={{ left: x, top: y }}
      >
        <button
          onClick={onAddNote}
          className="w-full px-3 py-2 text-left text-xs font-medium text-slate-700 hover:bg-slate-50"
        >
          📍 Not Ekle
        </button>
      </div>
    </>
  );
}