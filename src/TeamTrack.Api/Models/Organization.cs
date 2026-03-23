using System;

namespace TeamTrack.Api.Models;

public class Organization : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid OwnerUserId { get; set; }
    public User OwnerUser { get; set; } = null!;
}
