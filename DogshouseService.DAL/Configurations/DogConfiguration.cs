using DogshouseService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogshouseService.DAL.Configurations;

public class DogConfiguration : IEntityTypeConfiguration<Dog>
{
    public void Configure(EntityTypeBuilder<Dog> builder)
    {
        builder.ToTable("Dogs");


        builder.HasKey(dog => dog.Id);

        builder.Property(dog => dog.Id).ValueGeneratedOnAdd();


        builder.Property(dog => dog.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(dog => dog.Name, "IX_Dogs_Name")
            .IsUnique();


        builder.Property(dog => dog.Color)
            .HasColumnName("color")
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(dog => dog.TailLength)
            .HasColumnName("tail_length")
            .IsRequired();


        builder.Property(dog => dog.Weight)
            .HasColumnName("weight")
            .IsRequired();


        builder.ToTable(tb => tb.HasCheckConstraint(
            "CK_Dogs_TailLength_NonNegative",
            "[tail_length] >= 0"));

        builder.ToTable(tb => tb.HasCheckConstraint(
            "CK_Dogs_Weight_NonNegative",
            "[weight] >= 0"));
    }
}
