namespace TeamTrack.Api.Exceptions
{
    public class ForbiddenException(string message) : BaseException(message, 403)
    {
    }
}