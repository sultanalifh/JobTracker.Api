using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using Microsoft.OpenApi;

namespace JobTracker.Api.Services;

public class UserJobApplicationService : IUserJobApplicationService
{
    protected readonly IJobApplicationRepositories _repositories;
    protected readonly IStatisticsService _statistics;
    protected readonly ICurrentUserService _user;

    public UserJobApplicationService(IJobApplicationRepositories repositories, IStatisticsService statistics, ICurrentUserService user)
    {
        _repositories = repositories;
        _statistics = statistics;
        _user = user;
    }
    public async Task<JobApplicationPaginationResponse> GetMyApplicationsPage(GetApplicationRequest request)
    {
        int page = Math.Max(1, request.Page ?? 1);
        int pageSize = Math.Clamp(request.PageSize ?? 10, 1, 100);
        ApplicationStatus? status = null;
        string? keyword = request.Keyword;

        if (request.Status != null)
        {
            if (Enum.TryParse<ApplicationStatus>(request.Status, out ApplicationStatus s))
            {
                status = s;
            }
            else
            {
                throw new ValidationException("Invalid Status!");
            }
        }

        int totalApplications = await _repositories.CountAsync(_user.UserId, status, keyword);
        List<JobApplication> applications = await _repositories.GetAllByUserIdAsync(_user.UserId, page, pageSize, status, keyword);

        List<JobApplicationResponse> applicationsResponse = applications.Select(
            application => new JobApplicationResponse()
            {
                Id = application.Id,
                Company = application.Company,
                Position = application.Position,
                SiteLocation = application.SiteLocation,
                Status = application.Status.GetDisplayName(),
                CreatedAt = application.CreatedAt,
                UpdatedAt = application.UpdatedAt
            }).ToList();

        int totalPage = (int)Math.Ceiling(totalApplications / (pageSize + 0.0));
        int totalItems = totalApplications;

        return new JobApplicationPaginationResponse()
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPage,
            TotalItems = totalItems,
            Items = applicationsResponse
        };
    }

    public async Task<List<JobApplicationResponse>> GetMyApplications()
    {
        List<JobApplication> applications = await _repositories.GetAllByUserIdAsync(_user.UserId);

        List<JobApplicationResponse> applicationsResponse = applications.Select(
            application => new JobApplicationResponse()
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

    public async Task<JobApplicationResponse> GetMyApplication(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        if (_user.Role != UserRole.Admin && application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized Access!");
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

    public async Task<JobApplicationResponse> CreateMyApplication(CreateJobApplicationRequest request)
    {
        string company = request.Company;
        string position = request.Position;
        string siteLocation = request.SiteLocation;

        if (
            string.IsNullOrWhiteSpace(company) ||
            string.IsNullOrWhiteSpace(position) ||
            string.IsNullOrWhiteSpace(siteLocation))
        {
            throw new ValidationException("All field must be filled!");
        }

        JobApplication application = new JobApplication()
        {
            Company = company,
            Position = position,
            SiteLocation = siteLocation,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

            UserId = _user.UserId
        };

        await _repositories.AddAsync(application);

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

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

    public async Task<JobApplicationResponse> UpdateMyApplication(long id, UpdateJobApplicationRequest request)
    {
        string company = request.Company;
        string position = request.Position;
        string siteLocation = request.SiteLocation;
        ApplicationStatus status;

        if (
            string.IsNullOrWhiteSpace(company) ||
            string.IsNullOrWhiteSpace(position) ||
            string.IsNullOrWhiteSpace(siteLocation))
        {
            throw new ValidationException("All field must be filled!");
        }

        if (!Enum.TryParse<ApplicationStatus>(request.Status, out status))
        {
            throw new ValidationException("Invalid status!");
        }

        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        if (_user.Role != UserRole.Admin && application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized Access!");
        }

        application.Company = company;
        application.Position = position;
        application.SiteLocation = siteLocation;
        application.Status = status;
        application.UpdatedAt = DateTime.UtcNow;

        await _repositories.SavesChangesAsync();
        
        await InvalidateMyApplicationsStatistics();

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

    public async Task<JobApplicationStatusResponse> UpdateMyApplicationStatus(long id, UpdateJobApplicationStatusRequest request)
    {
        if(!Enum.TryParse<ApplicationStatus>(request.Status, out ApplicationStatus r))
        {
            throw new ValidationException("Invalid status!");
        }

        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        if (_user.Role != UserRole.Admin && application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized access!");
        }

        application.Status = request.NewStatus;
        application.UpdatedAt = DateTime.UtcNow;

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

        return new JobApplicationStatusResponse()
        {
            Id = id,
            Status = application.Status.GetDisplayName()
        };
    }

    public async Task<bool> DeleteMyApplication(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException();
        }

        if (_user.Role != UserRole.Admin && application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized Access!");
        }

        await _repositories.RemoveAsync(application);

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

        return true;
    }

    public async Task<StatisticsResponse> GetMyApplicationsStatistics() => await _statistics.GetStatisticsAsync(_user.UserId);

    public async Task InvalidateMyApplicationsStatistics() => await _statistics.InvalidateAsync(_user.UserId);
}