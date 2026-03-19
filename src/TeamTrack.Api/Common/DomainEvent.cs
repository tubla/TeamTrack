namespace TeamTrack.Api.Common
{
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}
