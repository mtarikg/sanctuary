using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sanctuary.API.Data;
using Sanctuary.API.Features.Auth;
using Sanctuary.API.Features.Categories;
using Sanctuary.API.Features.Notes;
using Sanctuary.API.Infrastructure;
using Sanctuary.API.Infrastructure.Constants;
using Sanctuary.API.Infrastructure.Extensions;
using Sanctuary.API.Infrastructure.Settings;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionSection)
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthService, LoggingAuthServiceDecorator>(sp =>
    new LoggingAuthServiceDecorator(
        sp.GetRequiredService<AuthService>(),
        sp.GetRequiredService<ILogger<LoggingAuthServiceDecorator>>()));
builder.Services.AddTransient<BearerSecuritySchemeTransformer>();

builder.Services.AddValidation();

builder.Services.AddProblemDetailsConfiguration();

builder.Services.AddOptions<JwtSettings>()
    .BindConfiguration(ConfigurationKeys.JwtSection).ValidateDataAnnotations().ValidateOnStart();

var jwtSettings = builder.Configuration.GetRequiredSection(ConfigurationKeys.JwtSection).Get<JwtSettings>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter(RateLimitingPolicies.Auth, o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(1);
    });

    opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.Configure<KestrelServerLimits>(opt =>
{
    opt.MaxRequestBodySize = 1_048_576; // 1MB
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(opt =>
{
    opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/test", () => "Hello, World! This is Sanctuary.");
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapCategoryEndpoints();
app.MapNoteEndPoints();
app.MapAuthEndpoints();

app.Run();

public partial class Program { } // Expose Program class for integration testing