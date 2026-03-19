using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTrack.Api.Models
{
    public class OrganizationUser : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}