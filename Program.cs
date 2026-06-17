using System.Text;
using JobTracker.Api.Data;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Middlewares;
using JobTracker.Api.Repositories;
using JobTracker.Api.Security;
using JobTracker.Api.Services;
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
builder.Services.AddAuthorization();

// Repositories
builder.Services.AddScoped<IJobApplicationRepositories, JobApplicationRepositories>();
builder.Services.AddScoped<IUserRepositories, UserRepositories>();

// Service
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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

app.MapPost("/register", async (IAuthService auth, RegisterRequest request) =>
{
    UserResponse response = await auth.Register(request);

    return response;
});

app.MapPost("/login", async (IAuthService auth, LoginRequest request) =>
{
    LoginResponse response = await auth.Login(request);

    return response;
});

// Feature

app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/applications", async (IJobApplicationService jobApplicationService) => Results.Ok(await jobApplicationService.GetMyApplications())).RequireAuthorization();

app.MapGet("/applications/{id}", async (IJobApplicationService jobApplicationService, long id) =>
{
    JobApplicationResponse? application = await jobApplicationService.GetMyApplication(id);

    return Results.Ok(application);
}
);

app.MapDelete("/applications/{id}", async (IJobApplicationService jobApplicationService, long id) =>
{
    await jobApplicationService.DeleteMyApplication(id);

    return Results.NoContent();
});

app.MapPatch("/applications/{id}/status", async (IJobApplicationService jobApplicationService, UpdateJobApplicationStatusRequest request, long id) =>
{
    JobApplicationStatusResponse? response = await jobApplicationService.SetMyAppliacionStatus(id, request);

    return Results.Ok(response);
});

app.MapPost("/applications", async (IJobApplicationService jobApplicationService, CreateJobApplicationRequest request) =>
{
    JobApplicationResponse application = await jobApplicationService.CreateMyApplication(request);

    return Results.Created($"/applications/{application.Id}", application);
});

app.MapGet("/statistics", async (IJobApplicationService jobApplicationService) => Results.Ok(await jobApplicationService.GetMyApplicationsStatistics()));

app.Run();