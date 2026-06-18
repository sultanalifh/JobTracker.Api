namespace JobTracker.Api.Dtos.Request;

public class GetApplicationRequest
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? Status { get; init; }
    public string? Keyword { get; init; }
}