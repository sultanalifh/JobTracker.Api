using JobTracker.Api.Dtos.Response;

namespace JobTracker.Api.Services;

public interface IStatisticsService
{
    Task<StatisticsResponse> GetStatisticsAsync();
    Task InvalidateAsync();
}