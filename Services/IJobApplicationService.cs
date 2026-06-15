using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Dtos.Request;

namespace JobTracker.Api.Services;
public interface IJobApplicationService
{
    Task<JobApplicationResponse> CreateAsync(CreateJobApplicationRequest dto);
    Task<List<JobApplicationResponse>> GetAllAsync();
    Task<JobApplicationResponse?> GetByIdAsync(long id);
    Task<JobApplicationStatusResponse?> GetStatusAsync(long id);
    Task<StatisticsResponse> GetStatisticsAsync();
    Task<JobApplicationStatusResponse?> SetStatusAsync(long id, UpdateJobApplicationStatusRequest dto);
    Task<bool> DeleteAsync(long id);
    Task InvalidateStatisticsAsync();
}