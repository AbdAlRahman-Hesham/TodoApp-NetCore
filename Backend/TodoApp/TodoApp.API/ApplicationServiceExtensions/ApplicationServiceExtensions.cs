using Mapster;
using TodoApp.Application.DTOs;

namespace TodoApp.Api.ServiceExtensions;


public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        TypeAdapterConfig<CreateTodoDto, TodoApp.Domain.Entities.Todo>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedDate)
            .Ignore(dest => dest.LastModifiedDate);

        TypeAdapterConfig<UpdateTodoDto, TodoApp.Domain.Entities.Todo>.NewConfig()
            .Ignore(dest => dest.CreatedDate)
            .Ignore(dest => dest.LastModifiedDate);

        services.AddSingleton(TypeAdapterConfig.GlobalSettings);

        return services;
    }
}


 
