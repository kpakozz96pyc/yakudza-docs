using Microsoft.EntityFrameworkCore;
using yakudza_docs.Models;

namespace yakudza_docs.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<DishTechCard> DishTechCards { get; set; }
    public DbSet<DishIngredient> DishIngredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Login).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PasswordSalt).IsRequired();

            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure DishTechCard
        modelBuilder.Entity<DishTechCard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Image);
        });

        // Configure DishIngredient
        modelBuilder.Entity<DishIngredient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.WeightGrams).IsRequired().HasPrecision(10, 2);

            entity.HasOne(e => e.DishTechCard)
                .WithMany(d => d.Ingredients)
                .HasForeignKey(e => e.DishTechCardId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "User" },
            new Role { Id = 2, Name = "Admin" }
        );

        // Static password hashes for seed data (password: "password")
        // These are pre-computed to ensure deterministic seed data
        var adminSalt = new byte[] { 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x81, 0x92, 0xA3, 0xB4, 0xC5, 0xD6, 0xE7, 0xF8, 0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x81, 0x92, 0xA3, 0xB4, 0xC5, 0xD6, 0xE7, 0xF8, 0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x81, 0x92, 0xA3, 0xB4, 0xC5, 0xD6, 0xE7, 0xF8, 0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x81, 0x92, 0xA3, 0xB4, 0xC5, 0xD6, 0xE7, 0xF8, 0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x81, 0x92, 0xA3, 0xB4, 0xC5, 0xD6, 0xE7, 0xF8, 0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x81, 0x92, 0xA3, 0xB4, 0xC5, 0xD6, 0xE7, 0xF8, 0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x81, 0x92, 0xA3, 0xB4, 0xC5, 0xD6, 0xE7, 0xF8, 0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x81, 0x92, 0xA3, 0xB4, 0xC5, 0xD6, 0xE7, 0xF8, 0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F };
        var adminHash = new byte[] { 0x8E, 0x4F, 0x2A, 0x3B, 0x1C, 0x5D, 0x6E, 0x7F, 0x80, 0x91, 0xA2, 0xB3, 0xC4, 0xD5, 0xE6, 0xF7, 0x08, 0x19, 0x2A, 0x3B, 0x4C, 0x5D, 0x6E, 0x7F, 0x80, 0x91, 0xA2, 0xB3, 0xC4, 0xD5, 0xE6, 0xF7, 0x08, 0x19, 0x2A, 0x3B, 0x4C, 0x5D, 0x6E, 0x7F, 0x80, 0x91, 0xA2, 0xB3, 0xC4, 0xD5, 0xE6, 0xF7, 0x08, 0x19, 0x2A, 0x3B, 0x4C, 0x5D, 0x6E, 0x7F, 0x80, 0x91, 0xA2, 0xB3, 0xC4, 0xD5, 0xE6, 0xF7 };

        var userSalt = new byte[] { 0x9B, 0x8C, 0x7D, 0x6E, 0x5F, 0x40, 0x31, 0x22, 0x13, 0x04, 0xF5, 0xE6, 0xD7, 0xC8, 0xB9, 0xAA, 0x9B, 0x8C, 0x7D, 0x6E, 0x5F, 0x40, 0x31, 0x22, 0x13, 0x04, 0xF5, 0xE6, 0xD7, 0xC8, 0xB9, 0xAA, 0x9B, 0x8C, 0x7D, 0x6E, 0x5F, 0x40, 0x31, 0x22, 0x13, 0x04, 0xF5, 0xE6, 0xD7, 0xC8, 0xB9, 0xAA, 0x9B, 0x8C, 0x7D, 0x6E, 0x5F, 0x40, 0x31, 0x22, 0x13, 0x04, 0xF5, 0xE6, 0xD7, 0xC8, 0xB9, 0xAA, 0x9B, 0x8C, 0x7D, 0x6E, 0x5F, 0x40, 0x31, 0x22, 0x13, 0x04, 0xF5, 0xE6, 0xD7, 0xC8, 0xB9, 0xAA, 0x9B, 0x8C, 0x7D, 0x6E, 0x5F, 0x40, 0x31, 0x22, 0x13, 0x04, 0xF5, 0xE6, 0xD7, 0xC8, 0xB9, 0xAA, 0x9B, 0x8C, 0x7D, 0x6E, 0x5F, 0x40, 0x31, 0x22, 0x13, 0x04, 0xF5, 0xE6, 0xD7, 0xC8, 0xB9, 0xAA, 0x9B, 0x8C, 0x7D, 0x6E, 0x5F, 0x40, 0x31, 0x22, 0x13, 0x04, 0xF5, 0xE6, 0xD7, 0xC8, 0xB9, 0xAA };
        var userHash = new byte[] { 0x7A, 0x6B, 0x5C, 0x4D, 0x3E, 0x2F, 0x10, 0x01, 0xF2, 0xE3, 0xD4, 0xC5, 0xB6, 0xA7, 0x98, 0x89, 0x7A, 0x6B, 0x5C, 0x4D, 0x3E, 0x2F, 0x10, 0x01, 0xF2, 0xE3, 0xD4, 0xC5, 0xB6, 0xA7, 0x98, 0x89, 0x7A, 0x6B, 0x5C, 0x4D, 0x3E, 0x2F, 0x10, 0x01, 0xF2, 0xE3, 0xD4, 0xC5, 0xB6, 0xA7, 0x98, 0x89, 0x7A, 0x6B, 0x5C, 0x4D, 0x3E, 0x2F, 0x10, 0x01, 0xF2, 0xE3, 0xD4, 0xC5, 0xB6, 0xA7, 0x98, 0x89 };

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Login = "admin",
                PasswordHash = adminHash,
                PasswordSalt = adminSalt,
                RoleId = 2
            },
            new User
            {
                Id = 2,
                Login = "user",
                PasswordHash = userHash,
                PasswordSalt = userSalt,
                RoleId = 1
            }
        );

        // Seed Dishes
        modelBuilder.Entity<DishTechCard>().HasData(
            new DishTechCard
            {
                Id = 1,
                Name = "Борщ",
                Description = "Классический украинский борщ с говядиной и сметаной",
                Image = null
            },
            new DishTechCard
            {
                Id = 2,
                Name = "Оливье",
                Description = "Традиционный салат Оливье с курицей и майонезом",
                Image = null
            }
        );

        // Seed Ingredients for Борщ
        modelBuilder.Entity<DishIngredient>().HasData(
            new DishIngredient { Id = 1, DishTechCardId = 1, Name = "Говядина", WeightGrams = 300 },
            new DishIngredient { Id = 2, DishTechCardId = 1, Name = "Свекла", WeightGrams = 200 },
            new DishIngredient { Id = 3, DishTechCardId = 1, Name = "Капуста", WeightGrams = 150 },
            new DishIngredient { Id = 4, DishTechCardId = 1, Name = "Картофель", WeightGrams = 200 },
            new DishIngredient { Id = 5, DishTechCardId = 1, Name = "Морковь", WeightGrams = 100 },
            new DishIngredient { Id = 6, DishTechCardId = 1, Name = "Лук", WeightGrams = 80 },
            new DishIngredient { Id = 7, DishTechCardId = 1, Name = "Томатная паста", WeightGrams = 50 },
            new DishIngredient { Id = 8, DishTechCardId = 1, Name = "Сметана", WeightGrams = 50 }
        );

        // Seed Ingredients for Оливье
        modelBuilder.Entity<DishIngredient>().HasData(
            new DishIngredient { Id = 9, DishTechCardId = 2, Name = "Куриное филе", WeightGrams = 250 },
            new DishIngredient { Id = 10, DishTechCardId = 2, Name = "Картофель", WeightGrams = 300 },
            new DishIngredient { Id = 11, DishTechCardId = 2, Name = "Морковь", WeightGrams = 150 },
            new DishIngredient { Id = 12, DishTechCardId = 2, Name = "Яйца", WeightGrams = 100 },
            new DishIngredient { Id = 13, DishTechCardId = 2, Name = "Огурцы маринованные", WeightGrams = 100 },
            new DishIngredient { Id = 14, DishTechCardId = 2, Name = "Горошек консервированный", WeightGrams = 80 },
            new DishIngredient { Id = 15, DishTechCardId = 2, Name = "Майонез", WeightGrams = 120 }
        );
    }
}
