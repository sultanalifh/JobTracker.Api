namespace JobTracker.Api.Dtos.Request;

public class CreateJobApplicationRequest
{
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string SiteLocation { get; set; } = string.Empty;
}