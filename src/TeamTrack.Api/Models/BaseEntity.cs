using System.ComponentModel.DataAnnotations.Schema;
using TeamTrack.Api.Common;

namespace TeamTrack.Api.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedAt { get; set; }

        [NotMapped]
        private readonly List<DomainEvent> _domainEvents = [];

        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

        public void AddDomainEvent(DomainEvent eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}