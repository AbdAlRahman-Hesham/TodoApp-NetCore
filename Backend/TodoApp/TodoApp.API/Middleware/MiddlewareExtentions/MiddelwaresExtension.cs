using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure.Persistence.Data;

namespace TodoApp.Api.Middleware.MiddlewareExtentions;

public static class MiddelwaresExtension
{

    public static async Task<IApplicationBuilder> UseUpdateDataBase(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();



        }
        return app;
    }
    

}