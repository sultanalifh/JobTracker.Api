using System.Linq.Expressions;
using JobTracker.Api.Data;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Repositories;

public class JobApplicationRepositories : IJobApplicationRepositories
{
    private readonly AppDbContext _context;

    public JobApplicationRepositories(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<JobApplication>> GetAllAsync()
    {
        List<JobApplication> applications = await _context.JobApplications.ToListAsync();

        return applications;
    }
    public async Task<List<JobApplication>> GetAllAsync(int page, int pageSize, ApplicationStatus? status, string? keyword)
    {
        if (keyword != null)
        {
            keyword = keyword.ToLower();
        }

        List<JobApplication> applications =
            await _context.JobApplications
            .Where(application =>
                (keyword == null ||
                application.Company.ToLower().Contains(keyword) ||
                application.Position.ToLower().Contains(keyword) ||
                application.SiteLocation.ToLower().Contains(keyword)) &&
                (application.Status == status || status == null))
            .OrderBy(application => application.CreatedAt)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync();

        return applications;
    }
    public async Task<List<JobApplication>> GetAllByUserIdAsync(long userId)
    {
        List<JobApplication> applications = await _context.JobApplications.Where(application => application.UserId == userId).ToListAsync();

        return applications;
    }
    public async Task<List<JobApplication>> GetAllByUserIdAsync(long userId, int page, int pageSize, ApplicationStatus? status, string? keyword)
    {
        if (keyword != null)
        {
            keyword = keyword.ToLower();
        }

        List<JobApplication> applications = await _context.JobApplications
            .Where(application => 
                application.UserId == userId &&
                (keyword == null || 
                application.Company.ToLower().Contains(keyword) ||
                application.Position.ToLower().Contains(keyword) || 
                application.SiteLocation.ToLower().Contains(keyword)) && 
                (application.Status == status || status == null))
            .OrderBy(application => application.CreatedAt)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync();
        
        return applications;
    }
    public async Task<JobApplication?> GetByIdAsync(long id)
    {
        JobApplication? application = await _context.JobApplications.FirstOrDefaultAsync(application => application.Id == id);

        return application;
    }

    public async Task<int> CountAsync(Expression<Func<JobApplication, bool>> predicate)
    {
        int total = await _context.JobApplications.CountAsync(predicate);

        return total;
    }
    public async Task AddAsync(JobApplication application) => await _context.JobApplications.AddAsync(application);

    public Task RemoveAsync(JobApplication application)
    {
        _context.JobApplications.Remove(application);

        return Task.CompletedTask;
    }

    public async Task SavesChangesAsync() => await _context.SaveChangesAsync();
}