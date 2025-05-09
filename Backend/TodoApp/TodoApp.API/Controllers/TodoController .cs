using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Controllers;
using TodoApp.Application.DTOs;
using TodoApp.Application.Errors;
using TodoApp.Application.Interfaces.Persistence;
using TodoApp.Application.Parameters;
using TodoApp.Application.Specifications;
using TodoApp.Domain.Entities;
using TodoApp.Domain.IdentityEntities;

namespace TodoApp.API.Controllers;

[Authorize]
public class TodoController : ApiBaseController
{
    private readonly IRepository<Todo> _todoRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<TodoController> _logger;

    public TodoController(
        IRepository<Todo> todoRepo,
        UserManager<ApplicationUser> userManager, ILogger<TodoController> logger)
    {
        _todoRepo = todoRepo;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TodoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TodoDto>>> GetTodos([FromQuery] TodoParameters parameters)
    {
        var user = await _userManager.GetUserAsync(User);
        var spec = new TodoSpecification(parameters, user!.Id);
        var todos = await _todoRepo.ListAsync(spec);

        return Ok(todos.Adapt<IReadOnlyList<TodoDto>>());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDto>> GetTodoById(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);

        var parameters = new TodoParameters();
        var spec = new TodoSpecification(parameters, user!.Id)
        {
            Criteria = t => t.Id == id
        };

        var todo = await _todoRepo.GetEntityWithSpecAsync(spec);
        if (todo == null)
        {
            return NotFound(new ApiError(404, "Todo not found"));
        }

        return Ok(todo.Adapt<TodoDto>());
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoDto>> CreateTodo(CreateTodoDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        var todo = dto.Adapt<Todo>();
        todo.UserId = user!.Id;
        todo.CreatedDate = DateTime.UtcNow;
        todo.LastModifiedDate = DateTime.UtcNow;

        await _todoRepo.AddAsync(todo);
        await _todoRepo.SaveChangesAsync();

        return Ok(todo.Adapt<TodoDto>());
    }

    [HttpPut]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TodoDto>> UpdateTodo(UpdateTodoDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        var parameters = new TodoParameters();
        var spec = new TodoSpecification(parameters, user!.Id)
        {
            Criteria = t => t.Id == dto.Id
        };

        var existing = await _todoRepo.GetEntityWithSpecAsync(spec);
        if (existing == null)
        {
            return NotFound(new ApiError(404, "Todo not found"));
        }

        dto.Adapt(existing);
        existing.LastModifiedDate = DateTime.UtcNow;

        _todoRepo.Update(existing);
        await _todoRepo.SaveChangesAsync();

        return Ok(existing.Adapt<TodoDto>());
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTodo(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);

        var parameters = new TodoParameters();
        var spec = new TodoSpecification(parameters, user!.Id)
        {
            Criteria = t => t.Id == id
        };

        var todo = await _todoRepo.GetEntityWithSpecAsync(spec);
        if (todo == null)
        {
            return NotFound(new ApiError(404, "Todo not found"));
        }

        _todoRepo.Delete(todo);
        await _todoRepo.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TodoDto>> MarkAsComplete(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new ApiError(401, "User not authenticated"));
        }

        var spec = new TodoSpecification(new TodoParameters(), user.Id)
        {
            Criteria = t => t.Id == id
        };

        await using var transaction = await _todoRepo.BeginTransactionAsync();
        try
        {
            var todo = await _todoRepo.GetEntityWithSpecAsync(spec);
            if (todo == null)
            {
                return NotFound(new ApiError(404, "Todo not found"));
            }

            todo.MarkAsComplete();

            await _todoRepo.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(todo.Adapt<TodoDto>());
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error marking todo {TodoId} as complete", id);
            return StatusCode(500, new ApiError(500, "An error occurred while completing the todo"));
        }
    }
}