using HappyFurnitureBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<ProductMaterial> ProductMaterials { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Contact> Contacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Password).IsRequired();
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasOne(c => c.Parent)
                  .WithMany(c => c.Children)
                  .HasForeignKey(c => c.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Slug).IsRequired();
        });

        // Configure ProductCategory many-to-many relationship
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });
            
            entity.HasOne(pc => pc.Product)
                  .WithMany(p => p.ProductCategories)
                  .HasForeignKey(pc => pc.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(pc => pc.Category)
                  .WithMany(c => c.ProductCategories)
                  .HasForeignKey(pc => pc.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Material entity
        modelBuilder.Entity<Material>(entity =>
        {
            entity.Property(e => e.Name).IsRequired();
        });

        // Configure ProductMaterial many-to-many relationship
        modelBuilder.Entity<ProductMaterial>(entity =>
        {
            entity.HasKey(pm => new { pm.ProductId, pm.MaterialId });
            
            entity.HasOne(pm => pm.Product)
                  .WithMany(p => p.ProductMaterials)
                  .HasForeignKey(pm => pm.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(pm => pm.Material)
                  .WithMany(m => m.ProductMaterials)
                  .HasForeignKey(pm => pm.MaterialId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductVariant entity
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasOne(pv => pv.Product)
                  .WithMany(p => p.ProductVariants)
                  .HasForeignKey(pv => pv.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductImage entity
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasOne(pi => pi.Product)
                  .WithMany(p => p.ProductImages)
                  .HasForeignKey(pi => pi.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure BaseEntity properties for all entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property("CreatedAt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                    
                modelBuilder.Entity(entityType.ClrType)
                    .Property("UpdatedAt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            }
        }
    }
}