using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using Microsoft.OpenApi;

namespace JobTracker.Api.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly IJobApplicationRepositories _repositories;
    private readonly IStatisticsService _statistics;

    public JobApplicationService(IJobApplicationRepositories repositories, IStatisticsService statistics)
    {
        _repositories = repositories;
        _statistics = statistics;
    }
    public async Task<JobApplicationResponse> CreateAsync(CreateJobApplicationRequest dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Company))
        {
            throw new ValidationException("Company is Required");
        }
        if (string.IsNullOrWhiteSpace(dto.Position))
        {
            throw new ValidationException("Position is required");
        }
        if (string.IsNullOrWhiteSpace(dto.SiteLocation))
        {
            throw new ValidationException("Site Location is required");
        }

        JobApplication application = new JobApplication()
        {
            Company = dto.Company,
            Position = dto.Position,
            SiteLocation = dto.SiteLocation,
            Status = ApplicationStatus.Applied,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repositories.AddAsync(application);

        await _repositories.SavesChangesAsync();

        return JobApplicationService.toDto(application)!;
    }

    public async Task<List<JobApplicationResponse>> GetAllAsync()
    {
        List<JobApplication> applications = await _repositories.GetAllAsync();

        List<JobApplicationResponse> applicationDtos = applications.Select(application => JobApplicationService.toDto(application)!).ToList();

        return applicationDtos;
    }

    public async Task<JobApplicationResponse?> GetByIdAsync(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if(application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        return JobApplicationService.toDto(application);
    }

    public async Task<JobApplicationStatusResponse?> GetStatusAsync(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        JobApplicationStatusResponse? statusDto = null;

        if (application != null)
        {
            statusDto = new JobApplicationStatusResponse()
            {
                Id = application.Id,
                Status = application.Status.GetDisplayName()
            };
        }
        else
        {
            throw new JobApplicationNotFoundException($"Job application with id = {id} was not found");
        }

        return statusDto;
    }

    public async Task<StatisticsResponse> GetStatisticsAsync() => await _statistics.GetStatisticsAsync();

    public async Task<JobApplicationStatusResponse?> SetStatusAsync(long id, UpdateJobApplicationStatusRequest dto)
    {
        if(!Enum.TryParse(dto.Status, out ApplicationStatus _))
        {
            throw new ValidationException("Invalid status!");
        }

        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        application.Status = dto.NewStatus;

        await _repositories.SavesChangesAsync();

        await _statistics.InvalidateAsync();

        return new JobApplicationStatusResponse()
        {
            Id = id,
            Status = dto.Status
        };
    }

    public async Task<bool> DeleteAsync(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        await _repositories.RemoveAsync(application);

        await _repositories.SavesChangesAsync();

        await _statistics.InvalidateAsync();

        return true;
    }

    public async Task InvalidateStatisticsAsync() => await _statistics.InvalidateAsync();

    private static JobApplicationResponse? toDto(JobApplication? application)
    {
        if (application == null)
        {
            return null;
        }

        return new JobApplicationResponse()
        {
            Id = application.Id,
            Company = application.Company,
            Position = application.Position,
            SiteLocation = application.SiteLocation,
            Status = application.Status.GetDisplayName(),
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }


}