namespace TalkToMe.Core.Exceptions;

public class BedrockServiceException : Exception
{
    public BedrockServiceException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}