namespace JobTracker.Api.Dtos.Response;
public class StatisticsResponse
{
    public int TotalApplications { get; set; }
    public int TotalApplied { get; set; }
    public int TotalViewed { get; set; }
    public int TotalInterviewed { get; set; }
    public int TotalRejected { get; set; }
    public int TotalOffered { get; set; }
}