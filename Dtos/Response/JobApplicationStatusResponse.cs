namespace JobTracker.Api.Dtos.Response;

public class JobApplicationStatusResponse
{
    public long Id { get; set; }
    public string Status { get; set; } = string.Empty;
}