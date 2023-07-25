using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:SqlServer"]);

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();

app.MapGet("/products", (ApplicationDbContext context) => {
    var products = context.Products.ToList();

    if (products == null)
        return Results.NotFound();

    return Results.Ok(products);
});

app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) => 
{
    var product = context.Products.Where(p => p.Id == id)
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .FirstOrDefault();

    if (product == null)
        return Results.NotFound();

    return Results.Ok(product);
});

app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) =>
{
    var category = context.Category.Where(c => c.Id == productRequest.CategoryId).FirstOrDefault();

    var product = new Product {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };

    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag{ Name = item});
        }
    }

    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) =>
{
    var product = context.Products.Where(p => p.Id == id)
        .Include(p => p.Tags)
        .FirstOrDefault();
    
    var category = context.Category.Where(c => c.Id == productRequest.CategoryId).FirstOrDefault();

    product!.Code = productRequest.Code;
    product!.Name = productRequest.Name;
    product!.Description = productRequest.Description;
    product!.Category = category!;

    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag{ Name = item });
        }
    }

    context.SaveChanges();
    return Results.NoContent();
});

app.MapDelete("/products/{id}", ([FromRoute] int id,  ApplicationDbContext context) =>
{
    var product = context.Products.Where(p => p.Id == id).FirstOrDefault();

    if (product == null)
        return Results.NotFound();
    
    context.Products.Remove(product);
    context.SaveChanges();

    return Results.Ok();
});

app.Run();