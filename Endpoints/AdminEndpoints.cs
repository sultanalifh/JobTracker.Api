using JobTracker.Api.Services;

namespace JobTracker.Api.Endpoints;

public static class AdminEndpoints
{
    public static RouteGroupBuilder MapAdminEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/users", async (IUserService userService) => Results.Ok(await userService.GetAllUsersAsync()));
        group.MapGet("/applications", async (IJobApplicationService applicationService) => Results.Ok(await applicationService.GetAllApplicationsAsync()));
        group.MapGet("statistics", async (IJobApplicationService applicationService) => Results.Ok(await applicationService.GetStatisticsAsync()));

        return group;
    }
}