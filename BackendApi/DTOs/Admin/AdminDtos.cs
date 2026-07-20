using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Admin;

public sealed record UserListDto(
    int    Id,
    string Email,
    bool   IsActive,
    IEnumerable<string> Roles,
    int?   TeamId,
    string? TeamName);

public sealed record AssignRoleDto(
    [Required] int RoleId);

public sealed record AssignPermissionDto(
    [Required] int PermissionId);

public sealed record CreateRoleDto(
    [Required, MaxLength(50)] string Name);

public sealed record UpdateUserDto(
    bool IsActive);

public sealed record TeamDto(int Id, string Name);
public sealed record AssignTeamDto(int? TeamId);  
public sealed record AssignTeamBulkDto(
    [Required] int[] UserIds,
    int? TeamId); 
public sealed record CreateTeamDto(
    [Required, MaxLength(100)] string Name);

public sealed record CreateEmployeeDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);
