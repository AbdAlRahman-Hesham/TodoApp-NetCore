using System.Linq.Expressions;
using TodoApp.Application.Parameters;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Specifications;

public class TodoSpecification : BaseSpecification<Todo>
{
    public TodoSpecification(TodoParameters parameters, string userId)
        : base(BuildCriteria(parameters, userId))
    {
        if (parameters.PageNumber > 0 && parameters.PageSize > 0)
        {
            ApplyPaging((parameters.PageNumber - 1) * parameters.PageSize, parameters.PageSize);
        }

        if (string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            parameters.SortBy = "duedate"; // Default sort
        }

        var sortBy = parameters.SortBy.ToLowerInvariant();
        var sortDescending = parameters.SortDescending;

        switch (sortBy)
        {
            case "title":
                ApplySort(t => t.Title, sortDescending);
                break;
            case "priority":
                ApplySort(t => t.Priority, sortDescending);
                break;
            case "status":
                ApplySort(t => t.Status, sortDescending);
                break;
            case "createddate":
                ApplySort(t => t.CreatedDate, sortDescending);
                break;
            default: // Default is DueDate
                ApplySort(t => t.DueDate ?? DateTime.MaxValue, sortDescending);
                break;
        }
    }

    private static Expression<Func<Todo, bool>> BuildCriteria(TodoParameters parameters, string userId)
    {
        var today = DateTime.UtcNow.Date;
        var criteria = PredicateBuilder.Create<Todo>(t => t.UserId == userId);

        if (parameters.Status.HasValue)
        {
            criteria = criteria.And(t => t.Status == parameters.Status.Value);
        }

        if (parameters.Priority.HasValue)
        {
            criteria = criteria.And(t => t.Priority == parameters.Priority.Value);
        }

        if (parameters.StartDate.HasValue)
        {
            var startDate = parameters.StartDate.Value.Date;
            criteria = criteria.And(t => t.DueDate.HasValue && t.DueDate.Value.Date >= startDate);
        }

        if (parameters.EndDate.HasValue)
        {
            var endDate = parameters.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            criteria = criteria.And(t => t.DueDate.HasValue && t.DueDate.Value.Date <= endDate);
        }

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            criteria = criteria.And(t =>
                t.Title.ToLower().Contains(searchTerm) ||
                (t.Description != null && t.Description.ToLower().Contains(searchTerm))
            );
        }

        if (parameters.IsOverdue == true)
        {
            criteria = criteria.And(t =>
                t.Status != TodoStatus.Completed &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date < today
            );
        }

        if (parameters.IsDueToday == true)
        {
            criteria = criteria.And(t =>
                t.Status != TodoStatus.Completed &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date == today
            );
        }

        if (parameters.IsDueThisWeek == true)
        {
            var endOfWeek = today.AddDays(7 - (int)today.DayOfWeek);
            criteria = criteria.And(t =>
                t.Status != TodoStatus.Completed &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date >= today &&
                t.DueDate.Value.Date <= endOfWeek
            );
        }

        return criteria;
    }
}
