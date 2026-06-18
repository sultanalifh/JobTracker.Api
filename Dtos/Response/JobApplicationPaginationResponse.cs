using System.Text.Json.Serialization;

namespace JobTracker.Api.Dtos.Response;

public class JobApplicationPaginationResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
    [JsonPropertyName("items")]
    public List<JobApplicationResponse> Items { get; set; } = new();
}