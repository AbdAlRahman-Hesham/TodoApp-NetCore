using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence.Configrations;

internal class TodoConfigration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

        builder.Property(e => e.Status)
                .HasConversion<string>();

        builder.Property(e => e.Priority)
                .HasConversion<string>();

        builder.HasOne(e => e.User)
                .WithMany(u => u.Todos)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}
