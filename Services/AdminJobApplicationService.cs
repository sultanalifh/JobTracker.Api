using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using Microsoft.OpenApi;

namespace JobTracker.Api.Services;

public class AdminJobApplicationService : UserJobApplicationService, IAdminJobApplicationService
{
    public AdminJobApplicationService(IJobApplicationRepositories repositories, IStatisticsService statistics, ICurrentUserService user) 
        : base(repositories, statistics, user)
    {
        
    }
    public async Task<JobApplicationPaginationResponse> GetApplicationsPage(GetApplicationRequest request)
    {
        int page = request.Page ?? 1;
        int pageSize = request.PageSize ?? 10;
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

        int totalApplications = await _repositories.CountAsync(status, keyword);
        List<JobApplication> applications = await _repositories.GetAllAsync(page, pageSize, status, keyword);

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
        int totalItems = applications.Count;

        return new JobApplicationPaginationResponse()
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPage,
            TotalItems = totalItems,
            Items = applicationsResponse
        };
    }

    public async Task<JobApplicationPaginationResponse> GetApplicationsPage(long id, GetApplicationRequest request)
    {
        int page = request.Page ?? 1;
        int pageSize = request.PageSize ?? 10;
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
        int totalItems = applications.Count;

        return new JobApplicationPaginationResponse()
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPage,
            TotalItems = totalItems,
            Items = applicationsResponse
        };
    }

    public async Task<List<JobApplicationResponse>> GetApplications()
    {
        List<JobApplication> applications = await _repositories.GetAllAsync();

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

    public async Task<List<JobApplicationResponse>> GetApplications(long id)
    {
        List<JobApplication> applications = await _repositories.GetAllByUserIdAsync(id);

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

    public async Task<JobApplicationResponse> UpdateApplication(long id, UpdateJobApplicationRequest request)
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

        application.Company = company;
        application.Position = position;
        application.SiteLocation = siteLocation;
        application.Status = status;

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
    public async Task<JobApplicationStatusResponse> UpdateApplicationStatus(long id, UpdateJobApplicationStatusRequest request)
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
        
        application.Status = request.NewStatus;

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

        return new JobApplicationStatusResponse()
        {
            Id = id,
            Status = application.Status.GetDisplayName()
        };
    }
    public async Task<bool> DeleteApplication(long id)
    {
        JobApplication? application = await _repositories.GetByIdAsync(id);

        if (application == null)
        {
            throw new JobApplicationNotFoundException();
        }

        if (application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized Access!");
        }

        await _repositories.RemoveAsync(application);

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

        return true;
    }
}