using Ingredients.Protos;
using JaegerTracing;
using Orders.Protos;

var builder = WebApplication.CreateBuilder(args);

builder.AddJaegerTracing();

// Add services to the container.
builder.Services.AddControllersWithViews();

var ingredientsUri = builder.Configuration.GetServiceUri("Ingredients")
    ?? new Uri("https://localhost:5003");

builder.Services.AddGrpcClient<IngredientsService.IngredientsServiceClient>(options =>
{
    options.Address = ingredientsUri;
});

var ordersUri = builder.Configuration.GetServiceUri("Orders")
                ?? new Uri("https://localhost:5005");

builder.Services.AddGrpcClient<OrderService.OrderServiceClient>(options =>
{
    options.Address = ordersUri;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
