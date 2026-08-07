using Microsoft.EntityFrameworkCore;
using Sanctuary.API.Data;
using Sanctuary.API.Features.Categories;
using Sanctuary.API.Features.Notes;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INoteService, NoteService>();

builder.Services.AddValidation();

builder.Services.AddProblemDetails(opt =>
{
    opt.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails.Status == StatusCodes.Status400BadRequest)
            context.ProblemDetails.Title = "Please complete all required fields.";
    };
}
);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/test", () => "Hello, World! This is Sanctuary.");

app.MapCategoryEndpoints();
app.MapNoteEndPoints();

app.Run();