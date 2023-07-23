using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:SqlServer"]);

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