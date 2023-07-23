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