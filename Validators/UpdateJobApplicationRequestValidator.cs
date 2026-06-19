using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Models;

namespace JobTracker.Api.Validators;

public class UpdateJobApplicationRequestValidator : IValidator<UpdateJobApplicationRequest>
{
    public void Validate(UpdateJobApplicationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Company) ||
            string.IsNullOrWhiteSpace(request.Position) ||
            string.IsNullOrWhiteSpace(request.SiteLocation))
        {
            throw new ValidationException("All fields must be filled!");
        }

        if (!Enum.TryParse<ApplicationStatus>(request.Status, out ApplicationStatus _))
        {
            throw new ValidationException("Invalid status!");
        }
    }
}