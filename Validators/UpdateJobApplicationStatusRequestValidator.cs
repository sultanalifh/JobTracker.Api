using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Models;

namespace JobTracker.Api.Validators;

public class UpdateJobApplicationStatusRequestValidator : IValidator<UpdateJobApplicationStatusRequest>
{
    public void Validate(UpdateJobApplicationStatusRequest request)
    {
        if(!Enum.TryParse<ApplicationStatus>(request.Status, out ApplicationStatus _))
        {
            throw new ValidationException("Invalid status!");
        }
    }
}