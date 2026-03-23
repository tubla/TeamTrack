using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Data.Configurations
{
    public class OrgAccessRequestConfiguration : IEntityTypeConfiguration<OrgAccessRequest>
    {
        public void Configure(EntityTypeBuilder<OrgAccessRequest> builder)
        {
            builder.ToTable("OrgAccessRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}