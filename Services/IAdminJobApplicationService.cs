using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;

namespace JobTracker.Api.Services;

public interface IAdminJobApplicationService : IUserJobApplicationService
{
    Task<JobApplicationPaginationResponse> GetApplicationsPage(GetApplicationRequest request);
    Task<JobApplicationPaginationResponse> GetApplicationsPage(long id, GetApplicationRequest request);
    Task<List<JobApplicationResponse>> GetApplications();
    Task<List<JobApplicationResponse>> GetApplications(long id);
    Task<JobApplicationResponse> UpdateApplication(long id, UpdateJobApplicationRequest request);
    Task<JobApplicationStatusResponse> UpdateApplicationStatus(long id, UpdateJobApplicationStatusRequest request);
    Task<bool> DeleteApplication(long id);
}