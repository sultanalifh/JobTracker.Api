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

    public async Task<JobApplicationPaginationResponse> GetApplicationsPage(long id, GetApplicationRequest request)
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

        int totalApplications = await _repositories.CountAsync(id, status, keyword);
        List<JobApplication> applications = await _repositories.GetAllByUserIdAsync(id, page, pageSize, status, keyword);

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

    public async Task<StatisticsResponse> GetStatistics() => await _statistics.GetStatisticsAsync(-1);
}