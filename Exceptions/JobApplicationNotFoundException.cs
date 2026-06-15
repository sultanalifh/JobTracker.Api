namespace JobTracker.Api.Exceptions;

public class JobApplicationNotFoundException : Exception
{
    public JobApplicationNotFoundException() : base() {}

    public JobApplicationNotFoundException(string message) : base(message) {}

    public JobApplicationNotFoundException(long id) : base($"Job application with id = {id} was not found") {}
}