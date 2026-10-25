using GestionProductosMvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

// Cliente tipado para que el MVC consuma su propia Web API (sección "Web API", Tema 2).
// La URL base se arma en cada solicitud a partir del esquema y el host actuales,
// porque la API vive en esta misma aplicación.
builder.Services.AddHttpClient<ProductosApiClient>((servicios, cliente) =>
{
    var contexto = servicios.GetRequiredService<IHttpContextAccessor>().HttpContext!;
    cliente.BaseAddress = new Uri($"{contexto.Request.Scheme}://{contexto.Request.Host}/api/productos/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
