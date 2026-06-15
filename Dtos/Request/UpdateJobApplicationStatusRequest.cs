using JobTracker.Api.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace JobTracker.Api.Dtos.Request;

public class UpdateJobApplicationStatusRequest
{
    public string Status { get; set; } = string.Empty;

    [SwaggerIgnore]
    public ApplicationStatus NewStatus
    {
        get
        {
            return (ApplicationStatus) Enum.Parse(typeof(ApplicationStatus), Status);
        }
    }
}