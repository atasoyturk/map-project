using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs;

public sealed record UserListDto(
    int    Id,
    string Email,
    bool   IsActive,
    IEnumerable<string> Roles);

public sealed record AssignRoleDto(
    [Required] int RoleId);

public sealed record AssignPermissionDto(
    [Required] int PermissionId);

public sealed record CreateRoleDto(
    [Required, MaxLength(50)] string Name);

public sealed record UpdateUserDto(
    bool IsActive);