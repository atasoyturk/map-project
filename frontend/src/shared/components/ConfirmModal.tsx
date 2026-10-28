import { Modal }  from "./ui/Modal";
import { Button } from "./ui/Button";

interface ConfirmModalProps {
  message:   string;
  onConfirm: () => void;
  onCancel:  () => void;
}

export function ConfirmModal({ message, onConfirm, onCancel }: ConfirmModalProps) {
  return (
    <Modal onClose={onCancel} className="w-[300px] rounded-[14px] px-7 py-6">
      <h2 className="mb-2 text-base font-semibold text-ink">Emin misiniz?</h2>
      <p className="mb-5 text-[13px] text-muted">{message}</p>
      <div className="flex gap-2.5">
        <Button className="flex-1" onClick={onCancel}>İptal</Button>
        <Button className="flex-1" variant="danger" onClick={onConfirm}>Sil</Button>
      </div>
    </Modal>
  );
}
