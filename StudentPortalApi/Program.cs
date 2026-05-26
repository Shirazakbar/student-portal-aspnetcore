using Microsoft.EntityFrameworkCore;
using StudentPortalApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<StudentPortalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

//app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseMiddleware<StudentPortalApi.Middleware.AdminAuthorizationMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Enable serving static files (HTML, CSS, JS)
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
