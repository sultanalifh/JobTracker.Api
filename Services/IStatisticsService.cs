using JobTracker.Api.Dtos.Response;

namespace JobTracker.Api.Services;

public interface IStatisticsService
{
    Task<StatisticsResponse> GetStatisticsAsync(long userId);
    Task InvalidateAsync(long userId);
}