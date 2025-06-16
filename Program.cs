
// Programa principal

using JaveragesLibrary.Services.Features.Mangas;
using JaveragesLibrary.Infrastructure.Repositories;
using JaveragesLibrary.Services.MappingsM; 

var builder = WebApplication.CreateBuilder(args);
// La siguiente línea ahora debería poder encontrar ResponseMappingProfile:
builder.Services.AddAutoMapper(typeof(ResponseMappingProfile).Assembly);

// Add services to the container.
builder.Services.AddScoped<MangaService>();
builder.Services.AddTransient<MangaRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Esto registrará todos los perfiles en el ensamblado que contiene ResponseMappingProfile
builder.Services.AddAutoMapper(typeof(ResponseMappingProfile).Assembly);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Red API",
        Description = "Una API para gestionar una increíble colección de mangas",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Tu Nombre/Equipo",
            Url = new Uri("https://tuwebsite.com")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MangaBot API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

