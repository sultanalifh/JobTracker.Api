using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Services;

namespace JobTracker.Api.Endpoints;

public static class UserEndpointss
{
    public static RouteGroupBuilder MapJobApplicationEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/applications", async ([AsParameters] GetApplicationRequest request, IUserJobApplicationService applicationService) => Results.Ok(await applicationService.GetMyApplicationsPage(request)));

        group.MapGet("/applications/{id}", async (IUserJobApplicationService applicationService, long id) =>
        {
            JobApplicationResponse? application = await applicationService.GetMyApplication(id);

            return Results.Ok(application);
        }
        );

        group.MapDelete("/applications/{id}", async (IUserJobApplicationService applicationService, long id) =>
        {
            await applicationService.DeleteMyApplication(id);

            return Results.NoContent();
        });

        // group.MapPatch("/applications/{id}/status", async (IUserJobApplicationService applicationService, UpdateJobApplicationStatusRequest request, long id) =>
        // {
        //     JobApplicationStatusResponse? response = await applicationService.SetMyAppliacionStatus(id, request);

        //     return Results.Ok(response);
        // });

        group.MapPost("/applications", async (IUserJobApplicationService applicationService, CreateJobApplicationRequest request) =>
        {
            JobApplicationResponse application = await applicationService.CreateMyApplication(request);

            return Results.Created($"/applications/{application.Id}", application);
        });

        group.MapGet("/statistics", async (IUserJobApplicationService applicationService) => Results.Ok(await applicationService.GetMyApplicationsStatistics()));

        return group;
    }
}