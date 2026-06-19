using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Mappers;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using JobTracker.Api.Validators;
using Microsoft.OpenApi;

namespace JobTracker.Api.Services;

public class UserJobApplicationService : IUserJobApplicationService
{
    protected readonly IJobApplicationRepositories _repositories;
    protected readonly IStatisticsService _statistics;
    protected readonly ICurrentUserService _user;


    // Validators

    protected readonly IValidator<CreateJobApplicationRequest> _createApplicationValidator;
    protected readonly IValidator<UpdateJobApplicationRequest> _updateApplicationValidator;
    protected readonly IValidator<UpdateJobApplicationStatusRequest> _updateApplicationStatusValidator;

    public UserJobApplicationService(
        IJobApplicationRepositories repositories, 
        IStatisticsService statistics, 
        ICurrentUserService user,
        IValidator<CreateJobApplicationRequest> createApplicationValidator,
        IValidator<UpdateJobApplicationRequest> updateApplicationValidator,
        IValidator<UpdateJobApplicationStatusRequest> updateApplicationStatusValidator)
    {
        _repositories = repositories;
        _statistics = statistics;
        _user = user;

        _createApplicationValidator = createApplicationValidator;
        _updateApplicationValidator = updateApplicationValidator;
        _updateApplicationStatusValidator = updateApplicationStatusValidator;
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
            application => application.ToResponse()).ToList();

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
            application => application.ToResponse()).ToList();

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

        return application.ToResponse();
    }

    public async Task<JobApplicationResponse> CreateMyApplication(CreateJobApplicationRequest request)
    {
        _createApplicationValidator.Validate(request);

        JobApplication application = request.CreateApplication(_user.UserId);

        await _repositories.AddAsync(application);

        await _repositories.SavesChangesAsync();

        await InvalidateMyApplicationsStatistics();

        return application.ToResponse();
    }

    public async Task<JobApplicationResponse> UpdateMyApplication(long id, UpdateJobApplicationRequest request)
    {
        _updateApplicationValidator.Validate(request);

        JobApplication? application = await _repositories.GetByIdAsync(id);

        ApplicationStatus status = Enum.Parse<ApplicationStatus>(request.Status);

        if (application == null)
        {
            throw new JobApplicationNotFoundException(id);
        }

        if (_user.Role != UserRole.Admin && application.UserId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Unauthorized Access!");
        }

        application.Company = request.Company;
        application.Position = request.Position;
        application.SiteLocation = request.SiteLocation;
        application.Status = status;
        application.UpdatedAt = DateTime.UtcNow;

        await _repositories.SavesChangesAsync();
        
        await InvalidateMyApplicationsStatistics();

        return application.ToResponse();
    }

    public async Task<JobApplicationStatusResponse> UpdateMyApplicationStatus(long id, UpdateJobApplicationStatusRequest request)
    {
        _updateApplicationStatusValidator.Validate(request);

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

        return request.ToResponse(id);
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