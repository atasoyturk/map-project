---
paths:
  - "frontend/src/features/map/**"
---
# Map rules

- Map logic (layers, interactions, clicks) lives in hooks (`useXxx`), not in large components.
  Clean up OpenLayers layers, interactions and listeners in the hook's cleanup.
- Geometry is sent/received as WKT in EPSG:4326; the map view is EPSG:3857 — transform explicitly.
- POI icons come from the category `Icon` field (sub-categories inherit via `buildCategoryIconMap`); never key icons on category IDs.
- Map data from growing tables (POIs) is loaded by current extent or on explicit action; never fetch all rows.
  Known violation: `usePoiLoader` fetches all of `tbl_poi_active` via WFS (do not copy).
