using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScaffoldDeneme.Models;
using Microsoft.OpenApi.Models; 
using ScaffoldDeneme.Repositories;
using ScaffoldDeneme.Mappings;
using FluentValidation.AspNetCore;
using FluentValidation;
using ScaffoldDeneme.Validators;
using StackExchange.Redis;
using RabbitMQ.Client; 

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısı
builder.Services.AddDbContext<MydatabaseContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25))));

// Redis bağlantısı
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;

if (string.IsNullOrEmpty(redisConnectionString))
{
    throw new InvalidOperationException("Redis connection string is not configured.");
}

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

// RedisService sınıfını bağımlılıklar ile kaydetme
builder.Services.AddSingleton<RedisService>();

// AutoMapper ve diğer servisler
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<INationalityRepository, NationalityRepository>();

// FluentValidation ve validatorlar
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<Student>, StudentValidator>();     
builder.Services.AddScoped<IValidator<Nationality>, NationalityValidator>();

// Swagger ve health kontrolleri
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ScaffoldProject API", Version = "v1" });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/healthz");

// Swagger UI'ı yalnızca geliştirme ortamında etkinleştirme
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScaffoldProject API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
