using JobTracker.Api.Models;

namespace JobTracker.Api.Dtos.Response;

public class JobApplicationResponse
{
    public long Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string SiteLocation { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}