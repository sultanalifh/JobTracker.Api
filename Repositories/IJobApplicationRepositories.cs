using System.Linq.Expressions;
using JobTracker.Api.Models;

namespace JobTracker.Api.Repositories;

public interface IJobApplicationRepositories : IRepositories
{
    Task<List<JobApplication>> GetAllAsync();
    Task<List<JobApplication>> GetAllAsync(int page, int pageSize, ApplicationStatus? status, string? keyword);
    Task<List<JobApplication>> GetAllByUserIdAsync(long userId);
    Task<List<JobApplication>> GetAllByUserIdAsync(long userId, int page, int pageSize, ApplicationStatus? status, string? keyword);
    Task<JobApplication?> GetByIdAsync(long id);
    Task<int> CountAsync(Expression<Func<JobApplication, bool>> predicate);
    Task<int> CountAsync(ApplicationStatus? status, string? keyword);
    Task<int> CountAsync(long userId, ApplicationStatus? status, string? keyword);
    Task AddAsync(JobApplication application);
    Task RemoveAsync(JobApplication application);
}