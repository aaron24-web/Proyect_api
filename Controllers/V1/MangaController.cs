// MangaController.cs

using AutoMapper; // Para IMapper
using JaveragesLibrary.Domain.Dtos; // Asumiendo que MangaDTO y MangaCreateDTO están aquí
using JaveragesLibrary.Domain.Entities;
using JaveragesLibrary.Services.Features.Mangas;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Para IEnumerable

namespace JaveragesLibrary.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class MangaController : ControllerBase
{
    private readonly MangaService _mangaService;
    private readonly IMapper _mapper;

    public MangaController(MangaService mangaService, IMapper mapper)
    {
        _mangaService = mangaService; // Corregido: this._mangaService a _mangaService
        _mapper = mapper;             // Corregido: this._mapper a _mapper
    }

    // GET api/v1/manga
    [HttpGet]
    public IActionResult GetAll()
    {
        var mangas = _mangaService.GetAll();
        // Asumiendo que tienes un MangaDTO.cs en JaveragesLibrary.Domain.Dtos
        var mangaDtos = _mapper.Map<IEnumerable<MangaDTO>>(mangas);
        
        return Ok(mangaDtos);
    }

    // GET api/v1/manga/{id}
    [HttpGet("{id}")] // Mantenemos {id} como en tu código, asume que el binding funciona para int
    public IActionResult GetById(int id)
    {
        var manga = _mangaService.GetById(id);

        // Este chequeo depende de cómo GetById indica "no encontrado".
        // Si GetById devuelve null cuando no encuentra, sería: if (manga == null)
        // Si GetById devuelve un Manga con Id=0 como indicativo:
        if (manga == null || manga.Id <= 0) // Chequeo más robusto
            return NotFound();

        var dto = _mapper.Map<MangaDTO>(manga);
        return Ok(dto);
    }

    // POST api/v1/manga
    [HttpPost]
    public IActionResult Add(MangaCreateDTO mangaDto) // Renombrado el parámetro para claridad
    {
        var entity = _mapper.Map<Manga>(mangaDto);

        // La lógica de ID aquí es muy simple y puede fallar en varios escenarios.
        // Idealmente, el repositorio o la base de datos se encargarían del ID.
        var mangas = _mangaService.GetAll(); // Esto puede ser ineficiente solo para contar
        var mangaId = (mangas.Any() ? mangas.Max(m => m.Id) : 0) + 1; // Una forma un poco más segura de obtener el siguiente ID

        entity.Id = mangaId;
        var newMangaWithId = _mangaService.Add(entity); // Asumiendo que MangaService.Add devuelve el Manga con el Id.
                                                        // Si _mangaService.Add es void, usa 'entity' para el mapeo a DTO.

        // Si _mangaService.Add devuelve el Manga con ID (recomendado):
        // var dto = _mapper.Map<MangaDTO>(newMangaWithId);
        // return CreatedAtAction(nameof(GetById), new { id = newMangaWithId.Id }, dto);

        // Si _mangaService.Add es void y has asignado el ID a 'entity':
        var dto = _mapper.Map<MangaDTO>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
    }

    // PUT api/v1/manga/{id}
    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] Manga mangaToUpdate) // Asumo que aquí recibes la entidad completa
    {
        // Si recibes un DTO para actualizar, deberías mapearlo a la entidad Manga primero.
        // Por ejemplo: [FromBody] MangaUpdateDTO mangaUpdateDto
        // var mangaEntityToUpdate = _mapper.Map<Manga>(mangaUpdateDto);
        // mangaEntityToUpdate.Id = id; // Asegurar que el ID de la ruta se usa.

        if (id != mangaToUpdate.Id) // Si mangaToUpdate es la entidad Manga
        {
            return BadRequest(new { Message = "El ID de la ruta no coincide con el ID del cuerpo." });
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = _mangaService.Update(mangaToUpdate);
        if (!success)
        {
            return NotFound(new { Message = $"Manga con ID {id} no encontrado para actualizar." });
        }
        return NoContent();
    }

    // DELETE api/v1/manga/{id}
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var success = _mangaService.Delete(id);
        if (!success)
        {
            return NotFound(new { Message = $"Manga con ID {id} no encontrado para eliminar." });
        }
        return NoContent();
    }
}