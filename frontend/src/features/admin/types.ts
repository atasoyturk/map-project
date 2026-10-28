export interface GeoPermissionDto {
  id:          number;
  name:        string;
  wktGeometry: string;
  isActive:    boolean;
}

export interface TeamDto {
  id:   number;
  name: string;
}

export interface RoleDto {
  id:   number;
  name: string;
  hasAdminAccess: boolean;
}

export interface UserDto {
  id:       number;
  email:    string;
  isActive: boolean;
  roles:    string[];
  teamId:   number | null;
  teamName: string | null;
  hasAdminAccess: boolean;
}

export interface AdminUserLookupEntry {
  id:    number;
  email: string;
}
