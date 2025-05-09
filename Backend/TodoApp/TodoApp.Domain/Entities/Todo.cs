
using TodoApp.Domain.Events;
using TodoApp.Domain.IdentityEntities;

namespace TodoApp.Domain.Entities;
public class Todo
{
    private readonly List<DomainEvent> _domainEvents = new();

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoStatus Status { get; set; } = TodoStatus.Pending;
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void MarkAsComplete()
    {
        if (Status != TodoStatus.Completed)
        {
            Status = TodoStatus.Completed;
            LastModifiedDate = DateTime.UtcNow;
            AddDomainEvent(new TodoCompletedEvent(Id, Title, UserId, Priority.ToString(), DueDate));
        }
    }


}