using Microsoft.EntityFrameworkCore;
using BackendApi.Entities.Auth;
using BackendApi.Entities.Geo;
using BackendApi.Entities.Team;
using BackendApi.Entities.Annotation;
using BackendApi.Entities.Poi;
using BackendApi.Entities.Transit;

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
    public DbSet<Team>       Teams       => Set<Team>();
    public DbSet<Annotation> Annotations => Set<Annotation>();
    public DbSet<PoiCategory> PoiCategories => Set<PoiCategory>();
    public DbSet<Poi>         Pois          => Set<Poi>();
    public DbSet<TransitRoute> TransitRoutes => Set<TransitRoute>();
    public DbSet<TransitStop>  TransitStops  => Set<TransitStop>();
    public DbSet<GeoPermissionEntity> GeoPermissions => Set<GeoPermissionEntity>();
    public DbSet<UserGeoPermission> UserGeoPermissions => Set<UserGeoPermission>();
    public DbSet<RoleGeoPermission> RoleGeoPermissions => Set<RoleGeoPermission>();

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
        
        
        // UserGeoPermission
        modelBuilder.Entity<UserGeoPermission>()
            .HasKey(ugp => new { ugp.UserId, ugp.GeoPermissionId });

        modelBuilder.Entity<UserGeoPermission>()
            .HasOne(ugp => ugp.User)
            .WithMany()
            .HasForeignKey(ugp => ugp.UserId);

        modelBuilder.Entity<UserGeoPermission>()
            .HasOne(ugp => ugp.GeoPermission)
            .WithMany(gp => gp.UserGeoPermissions)
            .HasForeignKey(ugp => ugp.GeoPermissionId);

        // RoleGeoPermission
        modelBuilder.Entity<RoleGeoPermission>()
            .HasKey(rgp => new { rgp.RoleId, rgp.GeoPermissionId });

        modelBuilder.Entity<RoleGeoPermission>()
            .HasOne(rgp => rgp.Role)
            .WithMany()
            .HasForeignKey(rgp => rgp.RoleId);

        modelBuilder.Entity<RoleGeoPermission>()
            .HasOne(rgp => rgp.GeoPermission)
            .WithMany(gp => gp.RoleGeoPermissions)
            .HasForeignKey(rgp => rgp.GeoPermissionId);

        // User → Team 
        modelBuilder.Entity<User>()
            .HasOne<Team>()
            .WithMany()
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Annotation → User
        modelBuilder.Entity<Annotation>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Annotation → Team 
        modelBuilder.Entity<Annotation>()
            .HasOne<Team>()
            .WithMany()
            .HasForeignKey(a => a.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Point/Line/Polygon → Team 
        modelBuilder.Entity<PointEntity>()
            .HasOne<Team>()
            .WithMany()
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<LineEntity>()
            .HasOne<Team>()
            .WithMany()
            .HasForeignKey(l => l.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PolygonEntity>()
            .HasOne<Team>()
            .WithMany()
            .HasForeignKey(pg => pg.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // PoiCategory self-referencing (Parent-Child)
        modelBuilder.Entity<PoiCategory>()
            .HasOne<PoiCategory>()
            .WithMany()
            .HasForeignKey(pc => pc.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Poi → PoiCategory
        modelBuilder.Entity<Poi>()
            .HasOne<PoiCategory>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Poi → User
        modelBuilder.Entity<Poi>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // TransitStop → TransitRoute
        modelBuilder.Entity<TransitStop>()
            .HasOne<TransitRoute>()
            .WithMany()
            .HasForeignKey(s => s.TransitRouteId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // TransitRoute → User
        modelBuilder.Entity<TransitRoute>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TransitStop → User
        modelBuilder.Entity<TransitStop>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        // Seed Data
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin"},
            new Role { Id = 2, Name = "Çalışan"},
            new Role { Id = 3, Name = "Stajyer"},
            new Role { Id = 4, Name = "Takım Lideri"},
            new Role { Id = 5, Name = "POI Operatorü" },
            new Role { Id = 6, Name = "Kullanıcı" },
            new Role { Id = 7, Name = "Ulaşım Operatörü"}
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "point_create",   Description = "Nokta oluşturma yetkisi"   },
            new Permission { Id = 2, Name = "line_create",    Description = "Çizgi oluşturma yetkisi"   },
            new Permission { Id = 3, Name = "polygon_create", Description = "Poligon oluşturma yetkisi" },
            new Permission { Id = 4, Name = "admin_access",   Description = "Admin paneli erişim yetkisi" },
            new Permission { Id = 5, Name = "annotation_create",Description = "Not/işaret ekleme yetkisi"    }, 
            new Permission { Id = 6, Name = "annotation_read",  Description = "Not geçmişini görüntüleme yetkisi" },
            new Permission { Id = 7, Name = "poi_create",           Description = "POI oluşturma/güncelleme yetkisi" },  
            new Permission { Id = 8, Name = "poi_read",             Description = "POI görüntüleme yetkisi" },            
            new Permission { Id = 9, Name = "poi_category_manage",  Description = "POI kategori ağacını yönetme yetkisi" },
            new Permission { Id = 10, Name = "transit_stop_create",   Description = "Durak oluşturma/güncelleme yetkisi" },
            new Permission { Id = 11, Name = "transit_stop_read",     Description = "Durak/güzergah görüntüleme yetkisi" },
            new Permission { Id = 12, Name = "transit_route_manage",  Description = "Güzergah yönetme yetkisi" },
            new Permission { Id = 13, Name = "area_scan",           Description = "Alan tarama analizi yapma yetkisi" },
            new Permission { Id = 14, Name = "location_analysis",   Description = "Konum analizi yapma yetkisi" },
            new Permission { Id = 15, Name = "heatmap_view",        Description = "Isı haritası görüntüleme yetkisi" }
        );

        // Admin
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 },
            new RolePermission { RoleId = 1, PermissionId = 5 },   
            new RolePermission { RoleId = 1, PermissionId = 6 },
            new RolePermission { RoleId = 1, PermissionId = 7 },   
            new RolePermission { RoleId = 1, PermissionId = 8 },   
            new RolePermission { RoleId = 1, PermissionId = 9 },  
            new RolePermission { RoleId = 1, PermissionId = 10 },
            new RolePermission { RoleId = 1, PermissionId = 11 },
            new RolePermission { RoleId = 1, PermissionId = 12 },
            new RolePermission { RoleId = 1, PermissionId = 13 },
            new RolePermission { RoleId = 1, PermissionId = 14 },
            new RolePermission { RoleId = 1, PermissionId = 15 }
        );

        // Worker
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 2 },
            new RolePermission { RoleId = 2, PermissionId = 3 },
            new RolePermission { RoleId = 2, PermissionId = 5 },   
            new RolePermission { RoleId = 2, PermissionId = 6 },
            new RolePermission { RoleId = 2, PermissionId = 7 },   
            new RolePermission { RoleId = 2, PermissionId = 8 },   
            new RolePermission { RoleId = 2, PermissionId = 10 },  
            new RolePermission { RoleId = 2, PermissionId = 11 },              
            new RolePermission { RoleId = 2, PermissionId = 13 },             
            new RolePermission { RoleId = 2, PermissionId = 14 },              
            new RolePermission { RoleId = 2, PermissionId = 15 }   
        );
        // Stajyer 
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 3, PermissionId = 1 },    
            new RolePermission { RoleId = 3, PermissionId = 2 },   
            new RolePermission { RoleId = 3, PermissionId = 3 },   
            new RolePermission { RoleId = 3, PermissionId = 5 }    
        );

        // Takım Lideri 
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 4, PermissionId = 1 },   
            new RolePermission { RoleId = 4, PermissionId = 2 },   
            new RolePermission { RoleId = 4, PermissionId = 3 },   
            new RolePermission { RoleId = 4, PermissionId = 5 },   
            new RolePermission { RoleId = 4, PermissionId = 6 },
            new RolePermission { RoleId = 4, PermissionId = 7 },   
            new RolePermission { RoleId = 4, PermissionId = 8 },   
            new RolePermission { RoleId = 4, PermissionId = 10 },  
            new RolePermission { RoleId = 4, PermissionId = 11 },  
            new RolePermission { RoleId = 4, PermissionId = 12 },  
            new RolePermission { RoleId = 4, PermissionId = 13 },  
            new RolePermission { RoleId = 4, PermissionId = 14 },  
            new RolePermission { RoleId = 4, PermissionId = 15 }   
        );
        // POI Operatorü
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 5, PermissionId = 7 },   // poi_create
            new RolePermission { RoleId = 5, PermissionId = 8 }    // poi_read — category_manage nto exist
        );

        // Kullanıcı
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 6, PermissionId = 8 },  // poi_read
            new RolePermission { RoleId = 6, PermissionId = 11 }  // transit_stop_read
        );

        // Ulaşım Operatörü
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 7, PermissionId = 10 }, // transit_stop_create
            new RolePermission { RoleId = 7, PermissionId = 11 }, // transit_stop_read
            new RolePermission { RoleId = 7, PermissionId = 12 }  // transit_route_manage
        );

        modelBuilder.Entity<PoiCategory>().HasData(
            new PoiCategory { Id = 1, Name = "Yeme İçme", ParentCategoryId = null },
            new PoiCategory { Id = 2, Name = "Restoran",  ParentCategoryId = 1    },
            new PoiCategory { Id = 3, Name = "Kafe",      ParentCategoryId = 1    },
            new PoiCategory { Id = 4, Name = "Turizm",    ParentCategoryId = null },
            new PoiCategory { Id = 5, Name = "Müze",      ParentCategoryId = 4    }
        );
    }
}