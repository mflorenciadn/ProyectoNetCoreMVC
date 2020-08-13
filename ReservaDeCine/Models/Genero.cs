using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC.ReservaDeCine.Models
{
    public class Genero
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo descripción es requerido")]
        [MaxLength(50, ErrorMessage = "La longitud máxima de 50 caracteres")]
        [MinLength(4, ErrorMessage = "La longitud mínima es de 4 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        [Display(Name = "Películas")]
        public List<PeliculaGenero> Peliculas { get; set; }


    }
}