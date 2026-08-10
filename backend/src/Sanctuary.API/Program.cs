using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Sanctuary.API.Data;
using Sanctuary.API.Features.Auth;
using Sanctuary.API.Features.Categories;
using Sanctuary.API.Features.Notes;
using Sanctuary.API.Infrastructure;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<BearerSecuritySchemeTransformer>();

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var configuration = builder.Configuration;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(opt =>
{
    opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/test", () => "Hello, World! This is Sanctuary.");

app.UseAuthentication();
app.UseAuthorization();

app.MapCategoryEndpoints();
app.MapNoteEndPoints();
app.MapAuthEndpoints();

app.Run();