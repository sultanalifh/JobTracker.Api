using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;

namespace JobTracker.Api.Validators;

public class CreateJobApplicationRequestValidator : IValidator<CreateJobApplicationRequest>
{
    public void Validate(CreateJobApplicationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Company) ||
            string.IsNullOrWhiteSpace(request.Position) ||
            string.IsNullOrWhiteSpace(request.Position))
        {
            throw new ValidationException("All fields must be filled!");
        }
    }
}