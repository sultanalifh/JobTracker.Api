namespace JobTracker.Api.Models;

public class JobApplication()
{
    public long Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string SiteLocation { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}