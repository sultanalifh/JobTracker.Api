using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Dtos.Request;

namespace JobTracker.Api.Services;
public interface IJobApplicationService
{
    // User

    Task<List<JobApplicationResponse>> GetMyApplications(GetApplicationRequest request);
    Task<JobApplicationResponse?> GetMyApplication(long id);
    Task<JobApplicationResponse> CreateMyApplication(CreateJobApplicationRequest request);
    Task<bool> DeleteMyApplication(long id);
    Task<JobApplicationStatusResponse> GetMyApplicationStatus(long id);
    Task<JobApplicationStatusResponse> SetMyAppliacionStatus(long id, UpdateJobApplicationStatusRequest request);
    Task<StatisticsResponse> GetMyApplicationsStatistics();
    Task InvalidateMyApplicationsStatistics();

    // Admin

    Task<List<JobApplicationResponse>> GetAllApplicationsAsync(GetApplicationRequest request);
    Task<StatisticsResponse> GetStatisticsAsync();
}