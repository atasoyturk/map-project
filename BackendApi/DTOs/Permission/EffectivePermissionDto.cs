namespace BackendApi.DTOs.Permission;

public sealed record EffectivePermissionDto(
    int    Id,
    string Name,
    string Description,
    bool   IsGranted,
    string Origin,      // "Role" or "User" 
    string? RoleName    
);