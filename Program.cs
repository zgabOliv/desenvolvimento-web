using DW01.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.Use(async (context, next) =>
{
    Console.WriteLine("Entrou no Middleware 1");

    await next();

    Console.WriteLine("Saiu do Middleware 1");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("Entrou no Middleware 2");
    if (context.Request.Path == "/bloqueado") 
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Acesso bloqueado.");
        return;
    }
    await next();
    Console.WriteLine("Saiu do Middleware 2");
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/api/ola", () =>
{
    return "Olá! Minha primeira Minimal API.";
});

app.MapGet("/api/produtos", () =>
{
    return new[]
    {
        new { Id = 1, Nome = "Notebook", Preco = 3500.00},
        new { Id = 2, Nome = "Mouse", Preco = 80.00},
        new { Id = 3, Nome = "Teclado", Preco = 150.00},

    };

});

app.MapGet("/api/produtos/{id}", (int id) =>
{
    if (id == 1)
    {

        return Results.Ok(new
        {
            Id = 1,
            Nome = "Notebook",
            Preco = 3500

        });

    }
    return Results.NotFound();
});


app.Run();
