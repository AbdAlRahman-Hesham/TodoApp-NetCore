using TodoApp.Api.Middleware.MiddlewareExtentions;
using TodoApp.Api.ServiceExtensions;
using TodoApp.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure services via extension methods
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
await app.UseUpdateDataBase();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
