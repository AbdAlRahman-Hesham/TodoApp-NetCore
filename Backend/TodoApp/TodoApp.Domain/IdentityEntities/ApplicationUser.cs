using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.Entities;

namespace TodoApp.Domain.IdentityEntities;

public class ApplicationUser : IdentityUser
{
    public ICollection<Todo> Todos { get; set; } = new List<Todo>();
}