namespace TodoApp.Application.DTOs;

public class UpdateTodoDto : CreateTodoDto
{
    public Guid Id { get; set; }
}
