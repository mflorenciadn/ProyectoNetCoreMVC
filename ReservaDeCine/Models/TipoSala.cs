using System;
using System.ComponentModel.DataAnnotations;

namespace MVC.ReservaDeCine.Models
{
    public class TipoSala
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(20, ErrorMessage = "La longitud máxima de 20 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima es de 2 caracteres")]
        [Display(Name = "Nombre")]
        public String Nombre { get; set; }


        [Required(ErrorMessage = "El campo Precio de Entrada es requerido")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El precio de la entrada no puede ser negativo")]
        [Display(Name = "Precio de entrada")]
        public decimal PrecioEntrada { get; set; }
    }
}
