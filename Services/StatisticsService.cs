namespace JobTracker.Api.Services;

using System.Text.Json;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using Microsoft.Extensions.Caching.Distributed;

public class StatisticsService : IStatisticsService
{
    private readonly IJobApplicationRepositories _repositories;
    private readonly IDistributedCache _cache;
    private readonly string _key = "statistics:applications";

    public StatisticsService(IJobApplicationRepositories repositories, IDistributedCache cache)
    {
        _repositories = repositories;
        _cache = cache;
    }

    public async Task<StatisticsResponse> GetStatisticsAsync(long userId)
    {
        string? cached = await _cache.GetStringAsync(_key);

        if(cached != null)
        {
            return JsonSerializer.Deserialize<StatisticsResponse>(cached)!;
        }

        var statistics = new StatisticsResponse()
        {
            TotalApplications = await _repositories.CountAsync(application => userId == -1 || application.UserId == userId),
            TotalApplied = await _repositories.CountAsync(application => (userId == -1 || application.UserId == userId) && application.Status == ApplicationStatus.Applied),
            TotalViewed = await _repositories.CountAsync(application => (userId == -1 || application.UserId == userId) && application.Status == ApplicationStatus.Viewed),
            TotalInterviewed = await _repositories.CountAsync(application => (userId == -1 || application.UserId == userId) && application.Status == ApplicationStatus.Interview),
            TotalOffered = await _repositories.CountAsync(application => (userId == -1 || application.UserId == userId) && application.Status == ApplicationStatus.Offer),
            TotalRejected = await _repositories.CountAsync(application => (userId == -1 || application.UserId == userId) && application.Status == ApplicationStatus.Rejected)
        };

        cached = JsonSerializer.Serialize(statistics);

        await _cache.SetStringAsync(_key, cached, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return statistics;
    }

    public async Task InvalidateAsync() => await _cache.RemoveAsync(_key);
}