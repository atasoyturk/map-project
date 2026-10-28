import { useState } from "react";
import { Modal }  from "../../../../shared/components/ui/Modal";
import { Button } from "../../../../shared/components/ui/Button";
import { Field }  from "../../../../shared/components/ui/Field";

interface AnnotationModalProps {
  onSave:   (noteText: string) => void;
  onCancel: () => void;
  isSaving: boolean;
  error?:   string | null;
}

export function AnnotationModal({ onSave, onCancel, isSaving, error }: AnnotationModalProps) {
  const [noteText, setNoteText] = useState("");

  function handleSave() {
    if (!noteText.trim()) return;
    onSave(noteText.trim());
  }

  return (
    <Modal onClose={onCancel} className="w-[360px] rounded-2xl px-8 py-7">
      <h2 className="mt-1 mb-5 text-lg font-semibold text-ink">Not Ekle</h2>

      <Field label="Not Metni" className={error ? "mb-2" : "mb-6"}>
        <textarea
          value={noteText}
          onChange={(e) => setNoteText(e.target.value)}
          placeholder="Bu konuma dair notunuz..."
          rows={4}
          className="resize-y"
        />
      </Field>

      {error && <p className="mb-4 text-xs text-red-600">{error}</p>}

      <div className="flex gap-2.5">
        <Button className="flex-1" onClick={onCancel} disabled={isSaving}>İptal</Button>
        <Button
          className={`flex-2 ${isSaving ? "bg-slate-800! text-muted!" : ""}`}
          variant="primary"
          onClick={handleSave}
          disabled={isSaving || !noteText.trim()}
        >
          {isSaving ? "Kaydediliyor..." : "Kaydet"}
        </Button>
      </div>
    </Modal>
  );
}
