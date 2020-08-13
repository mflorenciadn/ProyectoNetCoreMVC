using MVC.ReservaDeCine.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MVC.ReservaDeCine.Models
{
    public abstract class Usuario
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Rol")]
        public abstract Role Role { get; }


        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Nombre es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [RegularExpression("[a-zA-ZZñÑáéíóúÁÉÍÓÚ]*", ErrorMessage = "Formato inválido. El Nombre sólo admite letras")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo Apellido es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Apellido es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Apellido es de 2 caracteres")]
        [RegularExpression("[a-zA-ZñÑáéíóúÁÉÍÓÚ]*", ErrorMessage = "Formato inválido. El Apellido sólo admite letras")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }


        [Required(ErrorMessage = "El campo Email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "El campo Nombre de Usuario es requerido")]
        [MaxLength(20, ErrorMessage = "La longitud máxima de Usuario es de 20 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Contraseña")]
        public byte[] Password { get; set; }


        [Display(Name = "Fecha de último acceso")]
        public DateTime? FechaUltimoAcceso { get; set; }
    }
}
