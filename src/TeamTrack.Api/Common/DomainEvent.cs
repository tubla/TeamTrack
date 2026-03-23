namespace TeamTrack.Api.Common
{
    public abstract class DomainEvent
    {
        public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;
    }
}
