using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Villa_API.Data;
using Villa_API.DTO;
using Villa_API.Models;
using Villa_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(o =>
{
    o.CreateMap<Villa, VillaCreateDTO>().ReverseMap();
    o.CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
    o.CreateMap<Villa, VillaDTO>().ReverseMap();
    o.CreateMap<VillaUpdateDTO, VillaDTO>().ReverseMap();
    o.CreateMap<User, UserDTO>().ReverseMap();
});

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
}