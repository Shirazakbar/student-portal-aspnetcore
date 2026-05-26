using Microsoft.EntityFrameworkCore;
using StudentPortalApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// DATABASE CONFIGURATION
// If running on Render, it will look for a production connection string or fallback to a local database.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudentPortalContext>(options =>
    options.UseSqlServer(connectionString));

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

// ==========================================
// CORRECTED HTTP REQUEST PIPELINE ORDER
// ==========================================

// 1. Configure OpenAPI/Swagger tools for development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 2. Only redirect to HTTPS locally to prevent Render redirect loops
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 3. Enable serving static files FIRST so the frontend files load immediately
app.UseDefaultFiles();
app.UseStaticFiles();

// 4. Global Security Policy Configurations
app.UseCors("AllowAll");

// 5. Apply custom Admin Authorization middleware safely AFTER static files are served
app.UseMiddleware<StudentPortalApi.Middleware.AdminAuthorizationMiddleware>();
app.UseAuthorization();

// 6. Map API Controller endpoints last so they can receive fetch requests
app.MapControllers();

app.Run();
