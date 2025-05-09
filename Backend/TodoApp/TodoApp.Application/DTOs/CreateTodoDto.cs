
using System.ComponentModel.DataAnnotations;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.DTOs;

public class CreateTodoDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TodoStatus Status { get; set; } = TodoStatus.Pending;

    public TodoPriority Priority { get; set; } = TodoPriority.Medium;

    public DateTime? DueDate { get; set; }
}
