namespace JobTracker.Api.Exceptions;

public class MalformedTokenException : Exception
{
    public MalformedTokenException() : base("Token was not in correct format!")
    {
        
    }
}