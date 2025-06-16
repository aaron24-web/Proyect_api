// MangaRepository.cs

using System.Text.Json;
using JaveragesLibrary.Domain.Entities;
using Microsoft.Extensions.Configuration; // Necesario para IConfiguration
using System.IO; // 👇 AÑADE ESTA LÍNEA

namespace JaveragesLibrary.Infrastructure.Repositories;

public class MangaRepository
{
    private List<Manga> _mangas;
    private readonly string _filePath; // Es buena práctica hacerlo readonly si solo se asigna en el constructor

    public MangaRepository(IConfiguration configuration)
    {
        _filePath = configuration.GetValue<string>("dataBank") ?? string.Empty;
        _mangas = LoadData();
    }

    public IEnumerable<Manga> GetAll()
    {
        return _mangas;
    }

    // Considera cambiar el GetById para que devuelva Manga? (nullable) y retornar null si no se encuentra.
    // Esto haría más clara la lógica de "no encontrado" en las capas superiores.
    // Por ahora, lo dejamos como está para minimizar cambios, pero es una mejora.
    public Manga GetById(int id)
    {
        return _mangas.FirstOrDefault(manga => manga.Id == id)
                ?? new Manga // Este new Manga() tendrá Id = 0 por defecto si es int
                {
                    // Id = 0, // O un Id que indique que no existe, si tu modelo Manga lo permite
                    Title = string.Empty,
                    Author = string.Empty
                    // Asegúrate de que todas las propiedades requeridas estén aquí o tu app podría fallar
                };
    }

    public void Add(Manga manga) // Add puede seguir siendo void si no necesitas confirmación directa
    {
        var currentPath = GetCurrentFilePath();
        if (string.IsNullOrEmpty(currentPath) || !File.Exists(currentPath)) // Mejorado el chequeo de ruta
        {
            // Considera crear el archivo si no existe, o al menos loggear un error.
            // Por ahora, si no existe, simplemente no hace nada.
             if (string.IsNullOrEmpty(currentPath)) return; // No hacer nada si _filePath estaba vacío
            // Si quieres crear el archivo y/o directorio:
            // var dir = Path.GetDirectoryName(currentPath);
            // if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }
            
        _mangas.Add(manga);
        // 👇 USA GetCurrentFilePath() para escribir
        File.WriteAllText(currentPath, JsonSerializer.Serialize(_mangas));
    }

    // 👇 CAMBIA void A bool
    public bool Update(Manga updatedManga)
    {
        var currentPath = GetCurrentFilePath();
        if (string.IsNullOrEmpty(currentPath) || !File.Exists(currentPath))
            return false; // No se pudo actualizar porque el archivo no existe

        var index = _mangas.FindIndex(m => m.Id == updatedManga.Id);

        if (index != -1)
        {
            _mangas[index] = updatedManga;
            // 👇 USA GetCurrentFilePath() para escribir
            File.WriteAllText(currentPath, JsonSerializer.Serialize(_mangas));
            return true; // Actualización exitosa
        }
        return false; // Manga no encontrado para actualizar
    }

    // 👇 CAMBIA void A bool
    public bool Delete(int id)
    {
        var currentPath = GetCurrentFilePath();
        if (string.IsNullOrEmpty(currentPath) || !File.Exists(currentPath))
            return false; // No se pudo eliminar

        var itemsRemoved = _mangas.RemoveAll(m => m.Id == id);
        if (itemsRemoved > 0)
        {
            // 👇 USA GetCurrentFilePath() para escribir
            File.WriteAllText(currentPath, JsonSerializer.Serialize(_mangas));
            return true; // Eliminación exitosa
        }
        return false; // Manga no encontrado para eliminar
    }

    private string GetCurrentFilePath()
    {
        // Si _filePath está vacío o es nulo, no podemos combinarlo.
        if (string.IsNullOrEmpty(_filePath))
        {
            // Puedes loggear un error aquí o lanzar una excepción si es un estado inválido.
            return string.Empty; 
        }
        var currentDirectory = Directory.GetCurrentDirectory();
        return Path.Combine(currentDirectory, _filePath);
    }

    private List<Manga> LoadData()
    {
        var currentPath = GetCurrentFilePath();
        if (!string.IsNullOrEmpty(currentPath) && File.Exists(currentPath))
        {
            var jsonData = File.ReadAllText(currentPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return new List<Manga>(); // Archivo vacío
            try
            {
                return JsonSerializer.Deserialize<List<Manga>>(jsonData) ?? new List<Manga>();
            }
            catch (JsonException ex)
            {
                // Loggear el error de deserialización es importante
                Console.WriteLine($"Error deserializando JSON: {ex.Message}");
                return new List<Manga>(); // Retornar lista vacía en caso de error
            }
        }
        return new List<Manga>();
    }
}