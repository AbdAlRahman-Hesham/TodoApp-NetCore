
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Domain.Events;
[NotMapped]
public abstract class DomainEvent : INotification
{
    public DateTime OccurredOn { get; protected set; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}

public class TodoCompletedEvent : DomainEvent
{
    public Guid TodoId { get; }
    public string Title { get; }
    public string UserId { get; }
    public string Priority { get; }
    public DateTime? DueDate { get; }
    public DateTime CompletedDate { get; }

    public TodoCompletedEvent(Guid todoId, string title, string userId,
        string priority, DateTime? dueDate)
    {
        TodoId = todoId;
        Title = title;
        UserId = userId;
        Priority = priority;
        DueDate = dueDate;
        CompletedDate = DateTime.UtcNow;
    }
}
public class TodoCreatedEvent : DomainEvent
{
    public Guid TodoId { get; }
    public string Title { get; }
    public string UserId { get; }

    public TodoCreatedEvent(Guid todoId, string title, string userId)
    {
        TodoId = todoId;
        Title = title;
        UserId = userId;
    }
}

public class TodoUpdatedEvent : DomainEvent {
    public Guid TodoId { get; }
    public string Title { get; }
    public string UserId { get; }

    public TodoUpdatedEvent(Guid todoId, string title, string userId)
    {
        TodoId = todoId;
        Title = title;
        UserId = userId;
    }
}
public class TodoDeletedEvent : DomainEvent {
    public Guid TodoId { get; }
    public string Title { get; }
    public string UserId { get; }

    public TodoDeletedEvent(Guid todoId, string title, string userId)
    {
        TodoId = todoId;
        Title = title;
        UserId = userId;
    }

}