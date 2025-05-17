using Microsoft.EntityFrameworkCore;
using ROSE.Core.Models;

namespace ROSE.Infrastructure.Data;

public class ProfileDbContext : DbContext
{
    public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.ProfileName);
            entity.Property(e => e.ProfileName).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.EncryptedPassword).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.PasswordSalt).IsRequired();
            entity.Property(e => e.PasswordIV).IsRequired();
        });
    }

    public DbSet<Profile> Profiles { get; set; } = null!;
} 