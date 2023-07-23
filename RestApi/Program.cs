using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();

app.MapGet("/products", () => {
    var products = ProductRepository.GetAll();

    if (products == null)
        return Results.NotFound();

    return Results.Ok(products);
});

app.MapGet("/products/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);

    if (product == null)
        return Results.NotFound();

    return Results.Ok(product);
});

app.MapPost("/products", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product.Code);
});

app.MapPut("/products", (Product product) =>
{
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.NoContent();
});

app.MapDelete("/products/{code}", (string code) =>
{
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Delete(productSaved);
    
    return Results.Ok();
});

app.Run();

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public static class ProductRepository
{
    public static List<Product> Products { get; set; } = Products =  new List<Product>();

    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product)
    {
        Products.Add(product);
    }

    public static List<Product> GetAll()
    {
        return Products;
    }

    public static Product GetBy(string code)
    {
        return Products.FirstOrDefault(c => c.Code == code)!;
    }

    public static void Delete(Product product)
    {
        Products.Remove(product);
    }
}

public class ApplicationDbContext: DbContext {
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        modelBuilder.Entity<Product>()
            .Property(p => p.Name).HasMaxLength(40).IsRequired(false);
        modelBuilder.Entity<Product>()
            .Property(p => p.Code).HasMaxLength(20).IsRequired(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=Products;User ID=sa;Password=1q2w3e4r@#$");
}