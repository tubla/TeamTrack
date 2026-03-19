namespace TeamTrack.Api.Exceptions
{
    public class BaseException(string message, int statusCode) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }
}