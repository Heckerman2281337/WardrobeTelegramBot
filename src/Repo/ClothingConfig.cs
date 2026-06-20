using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using telegramBot.src.Entities;
namespace telegramBot.src.Repo
{
    internal class ClothingConfig : IEntityTypeConfiguration<ClothingItem>
    {
        public void Configure(EntityTypeBuilder<ClothingItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(n => n.Name)
                .IsRequired()
                .HasMaxLength(80);

            builder.Property(f => f.FileId)
                .IsRequired();

            builder.HasIndex(x => x.Id);
        }
    }
}
