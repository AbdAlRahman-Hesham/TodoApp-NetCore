using TodoApp.Api.Middleware.MiddlewareExtentions;
using TodoApp.Api.ServiceExtensions;
using TodoApp.API.Middleware;
using TodoApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure services via extension methods
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add email configuration
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));
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
app.UseCors(op =>
{
    op.WithOrigins("http://127.0.0.1:5500")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
});
await app.UseUpdateDataBase();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
