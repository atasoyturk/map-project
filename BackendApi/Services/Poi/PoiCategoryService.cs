using BackendApi.Data;
using BackendApi.DTOs.Poi;
using BackendApi.Entities.Poi;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Poi;

public sealed class PoiCategoryService : IPoiCategoryService
{
    private readonly AppDbContext _context;

    public PoiCategoryService(AppDbContext context) => _context = context;

    public async Task<PoiCategoryResponseDto> CreateAsync(PoiCategoryRequestDto request)
    {
        var category = new PoiCategory
        {
            Name             = request.Name,
            ParentCategoryId = request.ParentCategoryId,
        };

        _context.PoiCategories.Add(category);
        await _context.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task<PoiCategoryResponseDto?> UpdateAsync(int id, PoiCategoryRequestDto request)
    {
        var category = await _context.PoiCategories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category is null) return null;

        if (request.ParentCategoryId is not null &&
            await CreatesCycleAsync(id, request.ParentCategoryId.Value))
            throw new ArgumentException("Bu ebeveyn ataması döngüsel bir hiyerarşi oluşturur.");

        category.Name             = request.Name;
        category.ParentCategoryId = request.ParentCategoryId;
        category.ModifiedDate     = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ToDto(category);
    }

    private async Task<bool> CreatesCycleAsync(int categoryId, int candidateParentId)
    {
        var currentId = (int?)candidateParentId;
        var visited    = new HashSet<int>();

        while (currentId is not null)
        {
            if (currentId == categoryId) return true;

            if (!visited.Add(currentId.Value)) return true;

            currentId = await _context.PoiCategories
                .Where(c => c.Id == currentId)
                .Select(c => c.ParentCategoryId)
                .FirstOrDefaultAsync();
        }

        return false;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var category = await _context.PoiCategories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category is null) return false;

        var hasActiveChildren = await _context.PoiCategories
            .AnyAsync(c => c.ParentCategoryId == id && !c.IsDeleted);
        if (hasActiveChildren)
            throw new InvalidOperationException("Alt kategorisi olan bir kategori silinemez. Önce alt kategorileri silin veya taşıyın.");

        var hasActivePois = await _context.Pois
            .AnyAsync(p => p.CategoryId == id && !p.IsDeleted);
        if (hasActivePois)
            throw new InvalidOperationException("Bu kategoriye bağlı POI'ler var. Önce onları başka bir kategoriye taşıyın veya silin.");

        category.SoftDelete(userId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IList<PoiCategoryTreeDto>> GetTreeAsync()
    {
        var categories = await _context.PoiCategories
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        var byParent = categories.ToLookup(c => c.ParentCategoryId);

        List<PoiCategoryTreeDto> Build(int? parentId) =>
            byParent[parentId]
                .Select(c => new PoiCategoryTreeDto(c.Id, c.Name, Build(c.Id)))
                .ToList();

        return Build(null);
    }

    private static PoiCategoryResponseDto ToDto(PoiCategory entity) =>
        new(entity.Id, entity.Name, entity.ParentCategoryId);
}