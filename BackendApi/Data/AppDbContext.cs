using Microsoft.EntityFrameworkCore;
using BackendApi.Entities.Auth;
using BackendApi.Entities.Geo;
using BackendApi.Entities.Team;
using BackendApi.Entities.Annotation;
using BackendApi.Entities.Poi;
using BackendApi.Entities.Transit;
using BackendApi.Entities.Company;
using BackendApi.Data.Seed;

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
    public DbSet<CompanyCategory> CompanyCategories => Set<CompanyCategory>();
    public DbSet<Company>         Companies         => Set<Company>();
    public DbSet<Vehicle>         Vehicles          => Set<Vehicle>();
    public DbSet<CompanyRoute>    CompanyRoutes     => Set<CompanyRoute>();
    public DbSet<ShipmentRecord>  ShipmentRecords   => Set<ShipmentRecord>();

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
        
        // CompanyRoute (many-to-many: Company <-> TransitRoute)
        modelBuilder.Entity<CompanyRoute>()
            .HasKey(cr => new { cr.CompanyId, cr.TransitRouteId });

        modelBuilder.Entity<CompanyRoute>()
            .HasOne(cr => cr.Company)
            .WithMany(c => c.CompanyRoutes)
            .HasForeignKey(cr => cr.CompanyId);

        modelBuilder.Entity<CompanyRoute>()
            .HasOne(cr => cr.TransitRoute)
            .WithMany(r => r.CompanyRoutes)
            .HasForeignKey(cr => cr.TransitRouteId);

        // Company -> CompanyCategory
        modelBuilder.Entity<Company>()
            .HasOne<CompanyCategory>()
            .WithMany()
            .HasForeignKey(c => c.CompanyCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Vehicle -> Company
        modelBuilder.Entity<Vehicle>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // ShipmentRecord -> TransitRoute
        modelBuilder.Entity<ShipmentRecord>()
            .HasOne<TransitRoute>()
            .WithMany()
            .HasForeignKey(s => s.TransitRouteId)
            .OnDelete(DeleteBehavior.Restrict);

        // ShipmentRecord -> Vehicle
        modelBuilder.Entity<ShipmentRecord>()
            .HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(s => s.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

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

        // Poi.OsmId — one row per OSM element; manual POIs have no OsmId
        modelBuilder.Entity<Poi>()
            .HasIndex(p => p.OsmId)
            .IsUnique()
            .HasFilter("\"OsmId\" IS NOT NULL");

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

        // Geometry columns: SRID 4326 enforced by the column type; GiST indexes for spatial queries
        const string geometry4326 = "geometry(Geometry,4326)";
        modelBuilder.Entity<Annotation>()         .Property(a => a.Geometry)        .HasColumnType(geometry4326);
        modelBuilder.Entity<GeoPermissionEntity>().Property(g => g.BoundaryGeometry).HasColumnType(geometry4326);
        modelBuilder.Entity<TransitRoute>()       .Property(r => r.RouteGeometry)   .HasColumnType(geometry4326);
        modelBuilder.Entity<TransitStop>()        .Property(s => s.Geometry)        .HasColumnType(geometry4326);
        modelBuilder.Entity<Poi>()                .Property(p => p.Geometry)        .HasColumnType(geometry4326);
        modelBuilder.Entity<PointEntity>()        .Property(p => p.Geometry)        .HasColumnType(geometry4326);
        modelBuilder.Entity<LineEntity>()         .Property(l => l.Geometry)        .HasColumnType(geometry4326);
        modelBuilder.Entity<PolygonEntity>()      .Property(p => p.Geometry)        .HasColumnType(geometry4326);

        modelBuilder.Entity<Poi>()          .HasIndex(p => p.Geometry).HasMethod("gist");
        modelBuilder.Entity<PointEntity>()  .HasIndex(p => p.Geometry).HasMethod("gist");
        modelBuilder.Entity<LineEntity>()   .HasIndex(l => l.Geometry).HasMethod("gist");
        modelBuilder.Entity<PolygonEntity>().HasIndex(p => p.Geometry).HasMethod("gist");

        modelBuilder.Seed();
    }
}