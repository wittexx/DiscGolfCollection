// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using MyDiscgolfDiscs.Models;
using MyDiscgolfDiscs.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<DiscContext>();
builder.Services.AddScoped<DiscService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");
app.UseStaticFiles(); // For serving disc images
app.UseRouting();
app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DiscContext>();
    await context.Database.EnsureCreatedAsync();
}

Console.WriteLine("Disc Golf API started! Visit http://localhost:5000/swagger for API docs");
app.Run();
