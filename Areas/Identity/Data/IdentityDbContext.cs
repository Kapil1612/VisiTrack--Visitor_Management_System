using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisiTrack.Areas.Identity.Data;
using VisiTrack.Models.ViewModel;
using VisiTrack.Models;

namespace VisiTrack.Areas.Identity.Data;

public class IdentityDbContext : IdentityDbContext<AppUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().HasData(
           new IdentityRole
           {
               Id = "1",
               Name = "Admin",
               NormalizedName = "ADMIN"
           },
           new IdentityRole
           {
               Id = "2",
               Name = "Staff",
               NormalizedName = "STAFF"
           }
        );

        var hasher = new PasswordHasher<AppUser>();

        var adminUser = new AppUser
        {
            Id = "100",
            UserName = "admin@erp.com",
            NormalizedUserName = "ADMIN@ERP.COM",
            Email = "admin@erp.com",
            NormalizedEmail = "ADMIN@ERP.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

        builder.Entity<AppUser>().HasData(adminUser);

        // Assign Admin role to seeded Admin user
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            UserId = "100",
            RoleId = "1"
        });

        builder.ApplyConfiguration(new AppplicationUserEntityConfig());
    }
}

public class AppplicationUserEntityConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
    }
}