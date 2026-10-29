---
paths:
  - "frontend/src/**/*.tsx"
---
# UI kit

- Styling with Tailwind classes in new code; follow the look of existing components.
  Modals whose markup you change use `shared/components/ui/` (Modal, Button, Field); no inline style.
- No hand-written z-index; use `Z` from `shared/components/ui/zIndex.ts` via `style={{ zIndex: Z.x }}`
  (the only allowed inline style, since Tailwind classes can't take constants).
