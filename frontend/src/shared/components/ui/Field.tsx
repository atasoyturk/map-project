import type { ReactNode } from "react";

interface FieldProps {
  label:      string;
  children:   ReactNode;
  className?: string;
}

// Styles the input/textarea passed as children, so callers keep full control of its props.
const inputClasses =
  "[&_input,&_textarea]:w-full [&_input,&_textarea]:rounded-lg [&_input,&_textarea]:border [&_input,&_textarea]:border-line " +
  "[&_input,&_textarea]:px-3 [&_input,&_textarea]:py-2 [&_input,&_textarea]:text-sm [&_input,&_textarea]:text-ink " +
  "[&_input,&_textarea]:outline-none [&_input,&_textarea]:focus:border-indigo-500";

export function Field({ label, children, className = "" }: FieldProps) {
  return (
    <label className={`block ${inputClasses} ${className}`}>
      <span className="mb-1.5 block text-xs font-medium text-gray-700">{label}</span>
      {children}
    </label>
  );
}
