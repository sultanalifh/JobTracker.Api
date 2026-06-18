using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;

namespace JobTracker.Api.Services;

public interface IUserJobApplicationService
{
    Task<JobApplicationPaginationResponse> GetMyApplicationsPage(GetApplicationRequest request);
    Task<List<JobApplicationResponse>> GetMyApplications();
    Task<JobApplicationResponse> GetMyApplication(long id);
    Task<JobApplicationResponse> CreateMyApplication(CreateJobApplicationRequest request);
    Task<JobApplicationResponse> UpdateMyApplication(long id, UpdateJobApplicationRequest request);
    Task<JobApplicationStatusResponse> UpdateMyApplicationStatus(long id, UpdateJobApplicationStatusRequest request);
    Task<bool> DeleteMyApplication(long id);
    
    Task<StatisticsResponse> GetMyApplicationsStatistics();
    Task InvalidateMyApplicationsStatistics();
}