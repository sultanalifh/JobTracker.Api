using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Services;

namespace JobTracker.Api.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/applications", async ([AsParameters] GetApplicationRequest request, IUserJobApplicationService applicationService) 
            => Results.Ok(await applicationService.GetMyApplicationsPage(request)));

        group.MapPost("/applications", async (IUserJobApplicationService applicationService, CreateJobApplicationRequest request) =>
        {
            JobApplicationResponse application = await applicationService.CreateMyApplication(request);

            return Results.Created($"/applications/{application.Id}", application);
        });

        group.MapGet("/applications/{id}", async (IUserJobApplicationService applicationService, long id) 
            => Results.Ok(await applicationService.GetMyApplication(id)));

        group.MapPatch("/applications/{id}", async (IUserJobApplicationService applicationService, UpdateJobApplicationRequest request, long id) 
            => Results.Ok(await applicationService.UpdateMyApplication(id, request)));

        group.MapDelete("/applications/{id}", async (IUserJobApplicationService applicationService, long id) =>
        {
            await applicationService.DeleteMyApplication(id);

            return Results.NoContent();
        });

        group.MapPatch("/applications/{id}/status", async (IUserJobApplicationService applicationService, UpdateJobApplicationStatusRequest request, long id) =>
        {
            JobApplicationStatusResponse? response = await applicationService.UpdateMyApplicationStatus(id, request);

            return Results.Ok(response);
        });

        group.MapGet("/statistics", async (IUserJobApplicationService applicationService) => Results.Ok(await applicationService.GetMyApplicationsStatistics()));

        return group;
    }
}