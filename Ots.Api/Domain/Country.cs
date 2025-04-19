using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ots.Base;

namespace Ots.Api.Domain;

[Table("Country", Schema = "dbo")]
public class Country : BaseEntity
{
    public string Name { get; set; }
    public string IsoCode { get; set; }
    public string PhoneCode { get; set; }
    public string Flag { get; set; }
}

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedDate).IsRequired(true);
        builder.Property(x => x.UpdatedDate).IsRequired(false);
        builder.Property(x => x.InsertedUser).IsRequired(true).HasMaxLength(250);
        builder.Property(x => x.UpdatedUser).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IsoCode).IsRequired().HasMaxLength(3);
        builder.Property(x => x.PhoneCode).IsRequired().HasMaxLength(3);
        builder.Property(x => x.Flag).IsRequired().HasMaxLength(500);

        builder.HasIndex(x => x.IsoCode).IsUnique(true);
    }
}