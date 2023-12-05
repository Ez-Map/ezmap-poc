using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetTagById(Guid? userId, Guid id, CancellationToken token = default);

    void AddTag(TagCreateDto dto);
    void UpdateTag(Task<Tag?> dbTag, TagUpdateDto dto);

    Task DeleteTagAsync(Guid id, CancellationToken token = default);

    Task<List<Tag>> GetListTagAsync(Guid? userId, CancellationToken token = default);

    Task<List<Tag>> Search(Guid? userId, string keyword, CancellationToken token = default);
}

public class TagRepository : ITagRepository
{
    private readonly EzMapContext _dbContext;

    public TagRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tag?> GetTagById(Guid? userId, Guid id, CancellationToken token = default)
    {
        var tag = await _dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId,
            cancellationToken: token);

        return tag;
    }

    public void AddTag(TagCreateDto dto)
    {
        Tag tag = new Tag(dto.Name, dto.Description);
        _dbContext.Tags.Add(tag);
    }

    public void UpdateTag(Task<Tag?> dbTag, TagUpdateDto dto)
    {
        dbTag.Name = dto.Name;
        dbTag.Description = dto.Description;
    }

    public async Task DeleteTagAsync(Guid id, CancellationToken token = default)
    {
        Tag? tag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Id == id, cancellationToken: token);

        if (tag is not null)
        {
            tag.DeletedDate = DateTime.Now;
        }
    }

    public async Task<List<Tag>> GetListTagAsync(Guid? userId, CancellationToken token = default)
    {
        if (!userId.HasValue)
        {
            // Consider returning an empty list instead of null
            return new List<Tag>();
        }

        List<Tag> listTag = await _dbContext.Tags.Where(
            x => x.UserId == userId).ToListAsync(cancellationToken: token);

        return listTag;
    }

    public async Task<List<Tag>> Search(Guid? userId, string keyword, CancellationToken token = default)
    {
        var tags = _dbContext.Tags.Where(x => x.UserId == userId);

        if (!String.IsNullOrEmpty(keyword))
        {
            tags = tags.Where(x => keyword.ToLower().Contains(x.Name.ToLower()));
        }

        return await tags.ToListAsync(cancellationToken: token);
    }
}