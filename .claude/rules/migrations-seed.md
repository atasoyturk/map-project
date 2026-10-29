---
paths:
  - "BackendApi/Migrations/**"
  - "BackendApi/Data/**"
  - "BackendApi/Entities/**"
---
# Entities, migrations & seed

Schema, migration and seed changes need the user's OK first (root CLAUDE.md → Never do without asking).
- All entities inherit `BaseEntity` (Id, IsActive, IsDeleted, CreatedDate, ModifiedDate, ModifiedUserId).
- Exception: link tables (`UserRole`, `RolePermission`, `UserPermission`, `UserGeoPermission`, `RoleGeoPermission`, `CompanyRoute`) do not inherit `BaseEntity` and are hard-deleted; `Role` is hard-deleted too.
- New tables use `[Table("tbl_...")]`. Auth tables (`Users`, `Roles`, `Permissions`, `UserRoles`, `RolePermissions`, `UserPermissions`) keep their current names — do not rename.
- New geometry tables: GiST index, `geometry(<Type>,4326)` column, ST_IsValid CHECK for lines/polygons.
- Schema source of truth: `Entities/`, `Data/AppDbContext.cs` and `Data/Seed/`. Migration Designer/snapshot files are hidden from search (`.ignore`); open them by path only if needed.
