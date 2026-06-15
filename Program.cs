using JobTracker.Api.Data;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Middlewares;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using JobTracker.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
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
builder.Services.AddScoped<IJobApplicationRepositories, JobApplicationRepositories>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();

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

app.MapGet("/applications", async (IJobApplicationService jobApplicationService) => Results.Ok(await jobApplicationService.GetAllAsync()));

app.MapGet("/applications/{id}", async (IJobApplicationService jobApplicationService, long id) =>
{
    JobApplicationResponse? application = await jobApplicationService.GetByIdAsync(id);

    return Results.Ok(application);
}
);

app.MapDelete("/applications/{id}", async (IJobApplicationService jobApplicationService, long id) =>
{
    await jobApplicationService.DeleteAsync(id);

    return Results.NoContent();
});

app.MapPatch("/applications/{id}/status", async (IJobApplicationService jobApplicationService, UpdateJobApplicationStatusRequest request, long id) =>
{
    JobApplicationStatusResponse? response = await jobApplicationService.SetStatusAsync(id, request);

    return Results.Ok(response);
});

app.MapPost("/applications", async (IJobApplicationService jobApplicationService, CreateJobApplicationRequest request) =>
{
    JobApplicationResponse application = await jobApplicationService.CreateAsync(request);

    return Results.Created($"/applications/{application.Id}", application);
});

app.MapGet("/statistics", async (IJobApplicationService jobApplicationService) => Results.Ok(await jobApplicationService.GetStatisticsAsync()));

app.Run();

