using GoodHamburger.Api.Application.Interfaces;
using GoodHamburger.Api.Application.Services;
using GoodHamburger.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── Services ────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Good Hamburger API",
        Version = "v1",
        Description = "API REST para gestão de pedidos da lanchonete Good Hamburger 🍔"
    });
    // Include XML comments for richer Swagger docs
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// DI registrations
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<OrderService>();

// CORS — allow Blazor WASM dev server
builder.Services.AddCors(o => o.AddPolicy("BlazorDev", policy =>
    policy
        .WithOrigins("https://localhost:7001", "http://localhost:5001")
        .AllowAnyMethod()
        .AllowAnyHeader()
));

// ── Pipeline ────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseCors("BlazorDev");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Good Hamburger API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
