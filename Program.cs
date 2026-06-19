using System.Text;
using JobTracker.Api.Data;
using JobTracker.Api.Endpoints;
using JobTracker.Api.Middlewares;
using JobTracker.Api.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication().AddJwtBearer((options) =>
{
    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!));
    options.TokenValidationParameters.ValidateIssuer = false;
    options.TokenValidationParameters.ValidateAudience = false;
});
builder.Services.AddSwaggerGen((options) =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement()
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
builder.Services.AddStackExchangeRedisCache((options) =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "JobTracker:";
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization((options) =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
});

// Repositories
builder.Services.AddJobApplicationRepositories();
builder.Services.AddUserRepositories();

// Service

// Minimum Dependency
builder.Services.AddJwtService();
builder.Services.AddPasswordService();
builder.Services.AddUserService();
builder.Services.AddCurrentUserService();
builder.Services.AddStatisticsService();

builder.Services.AddAuthService();
builder.Services.AddUserJobApplicationService();
builder.Services.AddAdminJobApplicationService();

builder.Services.AddValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Authentication

app.MapAuthEndpoints();

// User

app.UseAuthentication().UseAuthorization();

app.MapGroup("/user")
    .RequireAuthorization()
    .MapUserEndpoints();

// Admin

app.MapGroup("/admin")
    .RequireAuthorization("AdminOnly")
    .MapAdminEndpoints();


app.Run();