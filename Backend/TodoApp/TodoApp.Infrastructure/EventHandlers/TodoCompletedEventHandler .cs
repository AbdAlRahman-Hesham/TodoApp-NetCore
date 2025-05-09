using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Events;
using TodoApp.Domain.IdentityEntities;

namespace TodoApp.Infrastructure.EventHandlers;

public class TodoCompletedEmailNotificationHandler : INotificationHandler<TodoCompletedEvent>
{
    private readonly ILogger<TodoCompletedEmailNotificationHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TodoCompletedEmailNotificationHandler(
        ILogger<TodoCompletedEmailNotificationHandler> logger,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task Handle(TodoCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending email notification for completed Todo: {TodoId}", notification.TodoId);

        try
        {
            var user = await _userManager.FindByIdAsync(notification.UserId);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                _logger.LogWarning("Could not find user or email for TodoCompletedEvent. UserId: {UserId}", notification.UserId);
                return;
            }

            await _emailService.SendEmailAsync(
                user.Email,
                "Todo Completed",
                $"Your todo item '{notification.Title}' has been marked as complete.");

            _logger.LogInformation("Email notification sent successfully for Todo: {TodoId}", notification.TodoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notification for completed Todo: {TodoId}", notification.TodoId);
        }
    }
}