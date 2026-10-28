using Microsoft.EntityFrameworkCore;
using BackendApi.Entities.Auth;
using BackendApi.Entities.Poi;

namespace BackendApi.Data.Seed;

public static class SeedData
{
    // Fixed so the model snapshot does not change on every build.
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Admin"},
            new Role { Id = 2, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Çalışan"},
            new Role { Id = 3, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Stajyer"},
            new Role { Id = 4, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Takım Lideri"},
            new Role { Id = 5, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "POI Operatörü" },
            new Role { Id = 6, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Kullanıcı" },
            new Role { Id = 7, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Ulaşım Operatörü"}
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "point_create",   Description = "Nokta oluşturma yetkisi"   },
            new Permission { Id = 2, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "line_create",    Description = "Çizgi oluşturma yetkisi"   },
            new Permission { Id = 3, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "polygon_create", Description = "Poligon oluşturma yetkisi" },
            new Permission { Id = 4, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "admin_access",   Description = "Admin paneli erişim yetkisi" },
            new Permission { Id = 5, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "annotation_create",Description = "Not/işaret ekleme yetkisi"    }, 
            new Permission { Id = 6, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "annotation_read",  Description = "Not geçmişini görüntüleme yetkisi" },
            new Permission { Id = 7, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "poi_create",           Description = "POI oluşturma/güncelleme yetkisi" },  
            new Permission { Id = 8, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "poi_read",             Description = "POI görüntüleme yetkisi" },            
            new Permission { Id = 9, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "poi_category_manage",  Description = "POI kategori ağacını yönetme yetkisi" },
            new Permission { Id = 10, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "transit_stop_create",   Description = "Durak oluşturma/güncelleme yetkisi" },
            new Permission { Id = 11, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "transit_stop_read",     Description = "Durak/güzergah görüntüleme yetkisi" },
            new Permission { Id = 12, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "transit_route_manage",  Description = "Güzergah yönetme yetkisi" },
            new Permission { Id = 13, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "area_scan",           Description = "Alan tarama analizi yapma yetkisi" },
            new Permission { Id = 14, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "location_analysis",   Description = "Konum analizi yapma yetkisi" },
            new Permission { Id = 15, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "heatmap_view",        Description = "Isı haritası görüntüleme yetkisi" }
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
            new RolePermission { RoleId = 3, PermissionId = 5 },
            new RolePermission { RoleId = 3, PermissionId = 6 },   
            new RolePermission { RoleId = 3, PermissionId = 8 }   
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
            new RolePermission { RoleId = 5, PermissionId = 6 },    // annotation_read
            new RolePermission { RoleId = 5, PermissionId = 7 },   // poi_create
            new RolePermission { RoleId = 5, PermissionId = 8 }    // poi_read
        );

        // Kullanıcı
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 6, PermissionId = 8 },  // poi_read
            new RolePermission { RoleId = 6, PermissionId = 11 }  // transit_stop_read
        );

        // Ulaşım Operatörü
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 7, PermissionId = 6 },  // annotation_read
            new RolePermission { RoleId = 7, PermissionId = 8 },   // poi_read
            new RolePermission { RoleId = 7, PermissionId = 10 }, // transit_stop_create
            new RolePermission { RoleId = 7, PermissionId = 11 }, // transit_stop_read
            new RolePermission { RoleId = 7, PermissionId = 12 }// transit_route_manage
        );

        modelBuilder.Entity<PoiCategory>().HasData(
            new PoiCategory { Id = 1, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Yeme İçme", ParentCategoryId = null },
            new PoiCategory { Id = 2, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Restoran",  ParentCategoryId = 1    },
            new PoiCategory { Id = 3, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Kafe",      ParentCategoryId = 1    },
            new PoiCategory { Id = 4, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Turizm",    ParentCategoryId = null },
            new PoiCategory { Id = 5, CreatedDate = SeedDate, ModifiedDate = SeedDate, Name = "Müze",      ParentCategoryId = 4    }
        );
    }
}
