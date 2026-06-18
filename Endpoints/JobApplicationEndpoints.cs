using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Services;

namespace JobTracker.Api.Endpoints;

public static class JobApplicationEndpoints
{
    public static void MapJobApplicationEndpoints(this WebApplication app)
    {
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
    }
}