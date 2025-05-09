public interface IDomainEventService
{
    Task PublishAsync(IEnumerable<TodoApp.Domain.Events.DomainEvent> domainEvents);
}