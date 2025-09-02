using System.ComponentModel.DataAnnotations;

namespace HoteleriaGes.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Contraseña { get; set; }

        [Required]
        public string Rol { get; set; } // admin, recepcionista, cliente
    }
}
