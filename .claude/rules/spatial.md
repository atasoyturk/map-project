---
paths:
  - "BackendApi/Services/**"
  - "BackendApi/Data/**"
  - "BackendApi/Helpers/GeometryConverter.cs"
---
# Spatial rules

- Filter in SQL on `geometry` with an index-usable predicate (Intersects / IsWithinDistance / &&); never load
  rows to filter in C#.
- Use `geography` only to measure (distance, area) rows that are already filtered; never in WHERE or ORDER BY over
  a whole table (the cast disables the GiST index).
- Nearest-N: KNN candidates (`<->`, LIMIT ~10) from the index, then re-rank by geography distance.
- No DB call per data row in a loop (small fixed loops, e.g. 3 bands, are fine).
