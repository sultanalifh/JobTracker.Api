using JobTracker.Api.Models;

namespace JobTracker.Api.Utilities;

public static class Utility
{
    public static bool Contains(this JobApplication application, string? keyword)
    {
        if(keyword != null)
        {
            keyword = keyword.ToLower();
        }

        bool verdict = 
            keyword == null ||
            application.Company.ToLower().Contains(keyword) ||
            application.Position.ToLower().Contains(keyword) ||
            application.SiteLocation.ToLower().Contains(keyword);
        
        return verdict;
    }

    public static bool StatusIsOrDefault(this JobApplication application, ApplicationStatus? status)
        => status == null || application.Status == status;
}