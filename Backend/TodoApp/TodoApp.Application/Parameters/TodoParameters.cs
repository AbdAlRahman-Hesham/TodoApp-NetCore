
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Parameters;

public class TodoParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    private string _sortBy = "DueDate";

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public TodoStatus? Status { get; set; }
    public TodoPriority? Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SearchTerm { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsDueToday { get; set; }
    public bool? IsDueThisWeek { get; set; }

    public string SortBy
    {
        get => _sortBy;
        set => _sortBy = !string.IsNullOrEmpty(value) ? value : "DueDate";
    }

    public bool SortDescending { get; set; } = false;
}