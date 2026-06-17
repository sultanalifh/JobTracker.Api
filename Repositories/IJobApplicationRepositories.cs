using System.Linq.Expressions;
using JobTracker.Api.Models;

namespace JobTracker.Api.Repositories;

public interface IJobApplicationRepositories : IRepositories
{
    Task<List<JobApplication>> GetAllAsync();
    Task<List<JobApplication>> GetAllByUserIdAsync(long userId);
    Task<JobApplication?> GetByIdAsync(long id);
    Task<int> CountAsync(Expression<Func<JobApplication, bool>> predicate);
    Task AddAsync(JobApplication application);
    Task RemoveAsync(JobApplication application);
}