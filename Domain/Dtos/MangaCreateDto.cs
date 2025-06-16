// MangaCreateDTO.cs

// Añade estos usings si no los tienes
using System; // Para DateTime
using System.ComponentModel.DataAnnotations; // Opcional, para validaciones como [Required]

namespace JaveragesLibrary.Domain.Dtos
{
    public class MangaCreateDTO
    {
        // Propiedades ya existentes
        [Required] // Opcional: añade validación si estos campos son obligatorios
        public string Title { get; set; } = null!;

        [Required] // Opcional: añade validación
        public string Author { get; set; } = null!;

        // --- ¡NUEVAS PROPIEDADES A AÑADIR! ---
        public string? Genre { get; set; } // Puede ser nullable si no es obligatorio

        [Required] // Opcional: añade validación si este campo es obligatorio
        public DateTime PublicationDate { get; set; }

        public int VolumeCount { get; set; }

        public bool IsOnApi { get; set; }
    }
}