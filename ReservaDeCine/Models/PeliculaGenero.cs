using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.ReservaDeCine.Models
{
    public class PeliculaGenero
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [ForeignKey("Pelicula")]
        [Display(Name = "Película")]
        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }


        [ForeignKey("Genero")]
        [Display(Name = "Género")]
        public int GeneroId { get; set; }
        public Genero Genero { get; set; }


    }
}
