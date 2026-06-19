namespace JobTracker.Api.Validators;

public interface IValidator<T>
{
    void Validate(T data);
}