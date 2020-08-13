using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.ReservaDeCine.Models
{
    public class Pelicula
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Nombre es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Display(Name = "Géneros")]
        public List<PeliculaGenero> Generos { get; set; }


        [MaxLength(250, ErrorMessage = "La longitud máxima de la sinopsis es de 250 caracteres")]
        [Display(Name = "Sinopsis")]
        public string Sinopsis { get; set; }


        [ForeignKey("Clasificacion")]
        [Required(ErrorMessage = "El campo Clasificación es requerido")]
        [Display(Name = "Clasificación")]
        public int ClasificacionId { get; set; }
        public Clasificacion Clasificacion { get; set; }


        [Display(Name = "Funciones")]
        public List<Funcion> Funciones { get; set; }


    }
}