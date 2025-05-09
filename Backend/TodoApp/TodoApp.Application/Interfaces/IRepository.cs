using Microsoft.EntityFrameworkCore.Storage;
using TodoApp.Application.Specifications;

namespace TodoApp.Application.Interfaces.Persistence;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
    public Task RollbackTransactionAsync();
    public Task CommitTransactionAsync();
    public Task<IDbContextTransaction>  BeginTransactionAsync();


}
