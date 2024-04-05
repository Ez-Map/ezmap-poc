using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EzMap.Domain.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetTagById(Guid? userId, Guid id, CancellationToken token = default);

    void AddTag(TagCreateDto dto);
    void UpdateTag(Tag? dbTag, TagUpdateDto dto);

    Task DeleteTagAsync(Guid id, CancellationToken token = default);

    Task<List<Tag>> GetListTagAsync(Guid? userId, CancellationToken token = default);

    Task<List<Tag>> Search(Guid? userId, string keyword, CancellationToken token = default);
}

public class TagRepository : ITagRepository
{
    private readonly EzMapContext _dbContext;

    private readonly ILogger<TagRepository> _logger;

    public TagRepository(EzMapContext dbContext, ILogger<TagRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Tag?> GetTagById(Guid? userId, Guid id, CancellationToken token = default)
    {
        var tag = await _dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId,
            cancellationToken: token);

        _logger.LogInformation($"Retrieved Tag with ID: {id} and User ID: {userId}");
        
        return tag;
    }

    public void AddTag(TagCreateDto dto)
    {
        Tag tag = new Tag(dto.Name, dto.Description, dto.UserId);
        _dbContext.Tags.Add(tag);
        _logger.LogInformation($"Added new Tag: {tag.Name} ({tag.Id}) for User ID: {tag.UserId}");
    }
    public void UpdateTag(Tag? dbTag, TagUpdateDto dto)
    {
        if (dbTag != null)
        {
            dbTag.Name = dto.Name;
            dbTag.Description = dto.Description;
            _logger.LogInformation($"Updated Tag: {dbTag.Name} ({dbTag.Id})");
        }
    }

    public async Task DeleteTagAsync(Guid id, CancellationToken token = default)
    {
        Tag? tag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Id == id, cancellationToken: token);

        if (tag is not null)
        {
            tag.DeletedDate = DateTime.Now;
            _logger.LogInformation($"Deleted Tag: {tag.Name} ({tag.Id})");
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
        
        _logger.LogInformation($"Returned a list: {listTag.Count} tag of User ID: {userId} ");

        return listTag;
    }

    public async Task<List<Tag>> Search(Guid? userId, string keyword, CancellationToken token = default)
    {
        var tags = _dbContext.Tags.Where(x => x.UserId == userId);

        if (!String.IsNullOrEmpty(keyword))
        {
            tags = tags.Where(x => keyword.ToLower().Contains(x.Name.ToLower()));
        }
        
        _logger.LogInformation($"Searched for Tags with keyword: {keyword} (User ID: {userId})");

        return await tags.ToListAsync(cancellationToken: token);
    }
}