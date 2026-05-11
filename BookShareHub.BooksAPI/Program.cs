using BookShareHub.Application.Books.Services;
using BookShareHub.Domain.Books.Repositories;
using BookShareHub.Infrastructure.Books.Repositories;
using Microsoft.OpenApi;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("BookShareHubDatabase")
    ?? throw new InvalidOperationException("Connection string 'BookShareHubDatabase' not found.");

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<IBookRepository>(sp => new BookRepository(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookShareHub API",
        Version = "v1",
        Description = """
        REST API for managing books.

        Features:
        - Create, update, retrieve and delete books
        - Borrow and return books
        """
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookShareHub API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
