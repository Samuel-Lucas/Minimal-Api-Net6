using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext: DbContext {
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Category { get; set; } = null!;
    public ApplicationDbContext(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        modelBuilder.Entity<Product>()
            .Property(p => p.Name).HasMaxLength(40).IsRequired(false);
        modelBuilder.Entity<Product>()
            .Property(p => p.Code).HasMaxLength(20).IsRequired(false);

        modelBuilder.Entity<Category>()
            .ToTable("Categories");
    }
}