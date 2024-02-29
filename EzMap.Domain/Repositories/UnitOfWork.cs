using System.Security.Claims;
using EzMap.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EzMap.Domain.Repositories;

public interface IUnitOfWork
{
    IPoiRepository PoiRepository { get; }
    IUserRepository UserRepository { get; }
    ITagRepository TagRepository { get; }
    IPoiCollectionRepository PoiCollectionRepository { get; }
    
    Task<int> SaveAsync();
    IDbContextTransaction BeginTransaction();
}

public class UnitOfWork : IDisposable ,IUnitOfWork
{
    private ILoggerFactory _loggerFactory;
    protected readonly EzMapContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private PoiRepository? _poiRepository;
    private UserRepository? _userRepository;
    private TagRepository? _tagRepository;
    private PoiCollectionRepository? _poiCollectionRepository;

    public UnitOfWork(ILoggerFactory loggerFactory, EzMapContext context, IHttpContextAccessor httpContextAccessor)
    {
        _loggerFactory = loggerFactory;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public IPoiCollectionRepository PoiCollectionRepository
    {
        get => _poiCollectionRepository ?? new PoiCollectionRepository(_context);
    }

    public ITagRepository TagRepository
    {
        get => _tagRepository ?? new TagRepository(_context);
    }

    public IPoiRepository PoiRepository
    {
        get => _poiRepository ?? new PoiRepository(_context);
    }

    public IUserRepository UserRepository
    {
        get => _userRepository ?? new UserRepository(_context, _loggerFactory.CreateLogger<UserRepository>());
    }

    public async Task<int> SaveAsync()
    {
        SetBaseAuditInfo();
        return await _context.SaveChangesAsync();
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }

    private void SetBaseAuditInfo()
    {
        var succssful = Guid.TryParse(_httpContextAccessor?.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier), out Guid tempId);

        var userId = succssful ? (Guid?)tempId : null;

        var entries = _context.ChangeTracker
            .Entries()
            .Where(e => e is { Entity: EntityBase<Guid>, State: (EntityState.Added or EntityState.Modified) });

        foreach (var entityEntry in entries)
        {
            var entity = entityEntry.Entity as EntityBase<Guid>;
            var utcNow = DateTime.UtcNow;

            entity.LastModifiedDate = utcNow;
            entity.LastModifiedBy = userId;

            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entity.DeletedDate = null;
                    entity.CreatedDate = utcNow;
                    entity.CreatedBy = userId ?? Guid.Empty;
                    break;
                case EntityState.Modified when entity.DeletedDate == null:
                    entity.LastModifiedDate = utcNow;
                    entity.LastModifiedBy = userId;
                    break;
            }
        }
    }

    public void Dispose()
    {
        _loggerFactory.Dispose();
        _context.Dispose();
    }
}