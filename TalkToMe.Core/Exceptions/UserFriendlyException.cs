namespace TalkToMe.Core.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public int StatusCode { get; }

        public UserFriendlyException(string message, int statusCode = 409) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
