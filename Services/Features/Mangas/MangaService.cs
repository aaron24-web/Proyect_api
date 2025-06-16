// MangaService.cs

using JaveragesLibrary.Domain.Entities;
using JaveragesLibrary.Infrastructure.Repositories;
using System.Collections.Generic; // Para IEnumerable

namespace JaveragesLibrary.Services.Features.Mangas;

public class MangaService
{
    private readonly MangaRepository _mangaRepository;

    public MangaService(MangaRepository mangaRepository)
    {
        _mangaRepository = mangaRepository; // Corregido: this._mangaRepository a _mangaRepository
    }

    public IEnumerable<Manga> GetAll()
    {
        return _mangaRepository.GetAll();
    }

    public Manga GetById(int id)
    {
        // Si cambias MangaRepository.GetById para que devuelva Manga? (nullable)
        // este método también debería idealmente devolver Manga?
        return _mangaRepository.GetById(id);
    }

    // 👇 CAMBIA void A Manga y retorna el manga
    public Manga Add(Manga manga)
    {
        _mangaRepository.Add(manga);
        // Asumimos que el objeto 'manga' pasado ya tiene un ID
        // o que el ID no es crítico para el Location header si es 0.
        // Si MangaRepository.Add asignara un ID, lo ideal sería que lo devolviera.
        return manga;
    }

    // 👇 CAMBIA void A bool y retorna el resultado del repositorio
    public bool Update(Manga mangaToUpdate)
    {
        // Eliminamos la pre-verificación con GetById aquí, ya que MangaRepository.Update
        // ahora devolverá true si el manga fue encontrado y actualizado, o false en caso contrario.
        return _mangaRepository.Update(mangaToUpdate);
    }

    // 👇 CAMBIA void A bool y retorna el resultado del repositorio
    public bool Delete(int id)
    {
        // Eliminamos la pre-verificación con GetById aquí por la misma razón.
        return _mangaRepository.Delete(id);
    }
}