using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TodoApp.Application.Interfaces.Persistence;
using TodoApp.Application.Specifications;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Data;
using TodoApp.Infrastructure.Services;


namespace TodoApp.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private protected readonly AppDbContext _context;
    private readonly IDomainEventService _domainEventService;

    public Repository(AppDbContext context, IDomainEventService domainEventService)
    {
        _context = context;
        _domainEventService = domainEventService;
    }

    public virtual async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(spec);
        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(spec);
        return await query.ToListAsync();
    }

    protected IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        var query = _context.Set<T>().AsQueryable();
        if (spec != null)
        {
            query = spec.Apply(query);  // Apply the specification logic
        }
        return query;
    }

    public async Task<T?> GetByIdAsync(Guid id)
        => await _context.Set<T>().FindAsync(id);

    public async Task<IReadOnlyList<T>> ListAllAsync()
        => await _context.Set<T>().ToListAsync();

    public async Task AddAsync(T entity)
        => await _context.Set<T>().AddAsync(entity);

    public void Update(T entity)
        => _context.Set<T>().Update(entity);

    public void Delete(T entity)
        => _context.Set<T>().Remove(entity);

    public async Task<int> SaveChangesAsync()
    {
        // Dispatch Domain Events before saving
        await DispatchDomainEventsAsync();

        return await _context.SaveChangesAsync();
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEventEntities = _context.ChangeTracker
            .Entries<Todo>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        // Get all domain events
        var domainEvents = domainEventEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        domainEventEntities.ForEach(e => e.ClearDomainEvents());

        await _domainEventService.PublishAsync(domainEvents);
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(spec);
        return await query.CountAsync();
    }
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}
