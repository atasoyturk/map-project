using Microsoft.EntityFrameworkCore;
using BackendApi.Entities.Auth;
using BackendApi.Entities.Geo;

namespace BackendApi.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>           Users      => Set<User>();
    public DbSet<Role>           Roles      => Set<Role>();
    public DbSet<Permission>     Permissions=> Set<Permission>();
    public DbSet<UserRole>       UserRoles  => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<PointEntity>    Points     => Set<PointEntity>();
    public DbSet<LineEntity>     Lines      => Set<LineEntity>();
    public DbSet<PolygonEntity>  Polygons   => Set<PolygonEntity>();
    public DbSet<GeoPermissionEntity> GeoPermissions => Set<GeoPermissionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.UseIdentityByDefaultColumns();

        // Composite PK 
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<UserPermission>()
            .HasKey(up => new { up.UserId, up.PermissionId });

        // Relationships
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        modelBuilder.Entity<UserPermission>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserPermissions)
            .HasForeignKey(up => up.UserId);

        modelBuilder.Entity<UserPermission>()
            .HasOne(up => up.Permission)
            .WithMany(p => p.UserPermissions)
            .HasForeignKey(up => up.PermissionId);
        
        modelBuilder.Entity<GeoPermissionEntity>()
            .HasOne(gp => gp.User)
            .WithMany()
            .HasForeignKey(gp => gp.UserId)
            .IsRequired(false);

        modelBuilder.Entity<GeoPermissionEntity>()
            .HasOne(gp => gp.Role)
            .WithMany()
            .HasForeignKey(gp => gp.RoleId)
            .IsRequired(false);

        // Seed Data
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User"  }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "point_create",   Description = "Nokta oluşturma yetkisi"   },
            new Permission { Id = 2, Name = "line_create",    Description = "Çizgi oluşturma yetkisi"   },
            new Permission { Id = 3, Name = "polygon_create", Description = "Poligon oluşturma yetkisi" },
            new Permission { Id = 4, Name = "admin_access",   Description = "Admin paneli erişim yetkisi" }
        );

        // Admin
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 }
        );

        // User
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 2 },
            new RolePermission { RoleId = 2, PermissionId = 3 }
        );
    }
}