using System.ComponentModel.DataAnnotations;
using System.Reflection;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using Microsoft.OpenApi;

namespace JobTracker.Api.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly ICurrentUserService _user;
    private readonly IJobApplicationRepositories _repositories;
    private readonly IStatisticsService _statistics;

    public JobApplicationService(ICurrentUserService user, IJobApplicationRepositories repositories, IStatisticsService statistics)
    {
        _user = user;
        _repositories = repositories;
        _statistics = statistics;
    }

    public async Task<JobApplicationResponse> CreateMyApplication(CreateJobApplicationRequest request)
    {
        string company = request.Company;
        string position = request.Position;
        string siteLocation = request.SiteLocation;

        if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(position) || string.IsNullOrWhiteSpace(siteLocation))
        {
            throw new ValidationException("All field must be filled!");
        }

        JobApplication application = new JobApplication()
        {
            Company = company,
            Position = position,
            SiteLocation = siteLocation,
            Status = ApplicationStatus.Applied,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = _user.UserId,
        };

        await _repositories.AddAsync(application);

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

        return new JobApplicationResponse()
        {
            Id = application.Id,
            Company = company,
            Position = position,
            SiteLocation = siteLocation,
            Status = application.Status.GetDisplayName(),
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }

    public async Task<bool> DeleteMyApplication(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        if (application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized to access!");
        }

        await _repositories.RemoveAsync(application);

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

        return true;
    }

    public async Task<List<JobApplicationResponse>> GetAllApplicationsAsync(GetApplicationRequest request)
    {
        int page = request.Page ?? 1;
        int pageSize = request.PageSize ?? 10;
        ApplicationStatus? status = null;
        string? keyword = request.Keyword;

        if(request.Status != null)
        {
            if(Enum.TryParse<ApplicationStatus>(request.Status, out ApplicationStatus s))
            {
                status = s;
            }
            else
            {
                throw new ValidationException("Invalid status!");
            }
        }
        List<JobApplication> applications = await _repositories.GetAllAsync(page, pageSize, status, keyword);

        List<JobApplicationResponse> applicationResponses = applications.Select(application => new JobApplicationResponse()
        {
            Id = application.Id,
            Company = application.Company,
            Position = application.Position,
            SiteLocation = application.SiteLocation,
            Status = application.Status.GetDisplayName(),
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        }).ToList();

        return applicationResponses;
    }

    public async Task<JobApplicationResponse?> GetMyApplication(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        if (application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized to access!");
        }

        return new JobApplicationResponse()
        {
            Id = id,
            Company = application.Company,
            Position = application.Position,
            SiteLocation = application.SiteLocation,
            Status = application.Status.GetDisplayName(),
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }

    public async Task<List<JobApplicationResponse>> GetMyApplications(GetApplicationRequest request)
    {
        int page = request.Page ?? 1;
        int pageSize = request.PageSize ?? 10;
        ApplicationStatus? status = null;
        string? keyword = request.Keyword;

        if(request.Status != null)
        {
            if(Enum.TryParse<ApplicationStatus>(request.Status, out ApplicationStatus s))
            {
                status = s;
            }
            else
            {
                throw new ValidationException("Invalid status!");
            }
        }

        List<JobApplication> applications = await _repositories.GetAllByUserIdAsync(_user.UserId, page, pageSize, status, keyword);
        
        List<JobApplicationResponse> applicationsResponse = applications.Select(application => new JobApplicationResponse()
        {
            Id = application.Id,
            Company = application.Company,
            Position = application.Position,
            SiteLocation = application.SiteLocation,
            Status = application.Status.GetDisplayName(),
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        }).ToList();

        return applicationsResponse;
    }

    public async Task<StatisticsResponse> GetMyApplicationsStatistics() => await _statistics.GetStatisticsAsync(_user.UserId);

    public async Task<JobApplicationStatusResponse> GetMyApplicationStatus(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        if (application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized to access!");
        }

        return new JobApplicationStatusResponse()
        {
            Id = id,
            Status = application.Status.GetDisplayName()
        };
    }

    public async Task<StatisticsResponse> GetStatisticsAsync() => await _statistics.GetStatisticsAsync(-1);

    public async Task InvalidateMyApplicationsStatistics() => await _statistics.InvalidateAsync(_user.UserId);

    public async Task<JobApplicationStatusResponse> SetMyAppliacionStatus(long id, UpdateJobApplicationStatusRequest request)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        if (application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized Access!");
        }

        if(!Enum.TryParse<ApplicationStatus>(request.Status, out ApplicationStatus result))
        {
            throw new ValidationException("Invalid status!");
        }

        application.Status = request.NewStatus;

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

        return new JobApplicationStatusResponse()
        {
            Id = id,
            Status = application.Status.GetDisplayName()
        };
    }
}