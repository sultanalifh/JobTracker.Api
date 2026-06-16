namespace JobTracker.Api.Exceptions;

public class InvalidTokenException : Exception
{
    public InvalidTokenException() : base("Token was invalid!")
    {
        
    }
}