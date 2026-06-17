namespace JobTracker.Api.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Wrong username / password!")
    {
        
    }

    public InvalidCredentialsException(string message) : base(message)
    {
        
    }
}