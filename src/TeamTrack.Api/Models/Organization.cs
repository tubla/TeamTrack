using System;

namespace TeamTrack.Api.Models
{
    public class Organization : BaseEntity
    {
        public string Name { get; set; }

        public Guid OwnerUserId { get; set; }
        public User OwnerUser { get; set; }
    }
}
