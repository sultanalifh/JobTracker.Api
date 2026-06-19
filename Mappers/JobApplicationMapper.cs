using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Models;
using Microsoft.OpenApi;

namespace JobTracker.Api.Mappers;

public static class JobApplicationMapper
{
    public static JobApplication CreateApplication(this CreateJobApplicationRequest request, long userId)
        => new JobApplication()
        {
            Company = request.Company,
            Position = request.Position,
            SiteLocation = request.SiteLocation,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

            UserId = userId
        };
    public static JobApplicationResponse ToResponse(this JobApplication application)
        => new JobApplicationResponse()
        {
            Id = application.Id,
            Company = application.Company,
            Position = application.Position,
            SiteLocation = application.SiteLocation,
            Status = application.Status.GetDisplayName(),
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt,
        };

    public static JobApplicationStatusResponse ToResponse(this UpdateJobApplicationStatusRequest request, long id)
        => new JobApplicationStatusResponse()
        {
            Id = id,
            Status = request.Status
        };

}