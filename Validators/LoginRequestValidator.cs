using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;

namespace JobTracker.Api.Validators;

public class LoginRequestValidator : IValidator<LoginRequest>
{
    public void Validate(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("All fields must be filled!");
        }
    }
}