using MVC.ReservaDeCine.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MVC.ReservaDeCine.Models
{
    public class Administrador : Usuario
    {
        [Required(ErrorMessage = "El campo Legajo es requerido")]
        [Display(Name = "Legajo")]
        public int Legajo { get; set; } //podria ser un Guid


        [ScaffoldColumn(false)]
        public override Role Role => Role.Administrador;
    }
}
