using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;

namespace JobTracker.Api.Validators;

public class RegisterRequestValidator : IValidator<RegisterRequest>
{
    public void Validate(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("All fields must be filled!");
        }
    }
}