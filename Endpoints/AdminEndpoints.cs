using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Services;

namespace JobTracker.Api.Endpoints;

public static class AdminEndpoints
{
    public static RouteGroupBuilder MapAdminEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/users", async (IUserService userService) => Results.Ok(await userService.GetAllUsersAsync()));
        group.MapGet("/applications", async ([AsParameters] GetApplicationRequest request, IAdminJobApplicationService applicationService) => Results.Ok(await applicationService.GetApplicationsPage(request)));
        group.MapGet("/applications/{id}", async ([AsParameters] GetApplicationRequest request, IAdminJobApplicationService applicationService, long id) => Results.Ok(await applicationService.GetApplicationsPage(id, request)));
        group.MapGet("/statistics", async (IAdminJobApplicationService applicationService) => Results.Ok(await applicationService.GetStatistics()));

        return group;
    }
}