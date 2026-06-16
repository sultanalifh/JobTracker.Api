namespace JobTracker.Api.Exceptions;

public class ExpiredTokenException : Exception
{
    public ExpiredTokenException() : base("Token expired!")
    {
        
    }
}