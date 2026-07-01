using Microsoft.EntityFrameworkCore;
using BackendApi.Entities;

namespace BackendApi.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    public DbSet<PointEntity>   Points   => Set<PointEntity>();
    public DbSet<LineEntity>    Lines    => Set<LineEntity>();
    public DbSet<PolygonEntity> Polygons => Set<PolygonEntity>();
}