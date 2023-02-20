using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "My first API in .NET6!");


app.MapPost("/saveproduct", (Product product) => {
    ProductRepository.add(product);
});

app.MapGet("/getproduct/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);
    return product;
});

app.MapPut("/editproduct", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
});

app.MapDelete("/deleteproduct/{code}", ([FromRoute] string code) => {
    var productDeleted = ProductRepository.GetBy(code);
    ProductRepository.Remove(productDeleted);
});

app.Run();

//simulating a database
public static class ProductRepository {
    public static List<Product> Products { get; set; }

    public static void add(Product product) {
        if(Products == null){
            Products = new List<Product>();
        }
        Products.Add(product);
    }
    public static Product GetBy(string code){
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product){
        Products.Remove(product);
    }

}

public class Product{
    public string Code { get; set; }
    public string Name { get; set; }
}
