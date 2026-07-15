using BackendApi.Entities;

namespace BackendApi.Helpers;

public static class EntityExtensions
{
    public static void SoftDelete(this BaseEntity entity, int userId)
    {
        entity.IsDeleted      = true;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;
    }
}