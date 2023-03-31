using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/", () => "My first API in .NET6!");


app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) => {
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category,
    };
    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapGet("/products/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);

    if(product != null){
        return Results.Ok(product);
    }

    return Results.NotFound();
});

app.MapPut("/products", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] string code) => {
    var productDeleted = ProductRepository.GetBy(code);
    ProductRepository.Remove(productDeleted);
    return Results.Ok();
});

//Applying rule for this code to run only in configured environment i.e. Stagings
if(app.Environment.IsStaging()){
    app.MapGet("/configuration/database", (IConfiguration configuration) => {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });
}

app.Run();