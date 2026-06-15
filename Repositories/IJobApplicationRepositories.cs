using System.Linq.Expressions;
using JobTracker.Api.Models;

namespace JobTracker.Api.Repositories;

public interface IJobApplicationRepositories
{
    Task<List<JobApplication>> GetAllAsync();
    Task<JobApplication?> GetByIdAsync(long id);
    Task<int> CountAsync(Expression<Func<JobApplication, bool>> predicate);
    Task AddAsync(JobApplication application);
    Task RemoveAsync(JobApplication application);
    Task SavesChangesAsync();
}