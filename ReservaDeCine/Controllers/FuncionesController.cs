using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.ReservaDeCine.Database;
using MVC.ReservaDeCine.Models;
using MVC.ReservaDeCine.Models.Enums;
using System;
using System.Linq;

namespace MVC.ReservaDeCine.Controllers
{

    public class FuncionesController : Controller
    {
        private readonly CineDbContext _context;

        public FuncionesController(CineDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Index()
        {
            var funciones = _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .ToList();

            return View(funciones);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var funcion = _context.Funciones
                .Include(x => x.Reservas).ThenInclude(x => x.Cliente)
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .FirstOrDefault(m => m.Id == id);

            if (funcion == null)
            {
                return NotFound();
            }

            return View(funcion);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Create()
        {
            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");

            return View();
        }


        [HttpPost]
        [Authorize(Roles = nameof(Role.Administrador))]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Funcion funcion)
        {

            ValidarFecha(funcion);
            ValidarHorario(funcion);

            if (ModelState.IsValid)
            {
                var sala = _context.Salas
                             .Where(x => x.Id == funcion.SalaId)
                             .FirstOrDefault();

                funcion.CantButacasDisponibles = sala.CapacidadTotal;

                _context.Add(funcion);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");

            return View(funcion);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = _context.Funciones.Find(id);

            if (funcion == null)
            {
                return NotFound();
            }

            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");

            return View(funcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Edit(int id, Funcion funcion)
        {

            if (id != funcion.Id)
            {
                return NotFound();
            }

            ValidarFecha(funcion);
            ValidarHorario(funcion);
            ValidarCapacidadNuevaSalaSegunReservas(id, funcion);

            if (ModelState.IsValid)
            {
                try
                {
                    var funcionDb = _context.Funciones.Find(id);

                    funcionDb.SalaId = funcion.SalaId;
                    funcionDb.PeliculaId = funcion.PeliculaId;
                    funcionDb.Fecha = funcion.Fecha;
                    funcionDb.Horario = funcion.Horario;

                    _context.Update(funcionDb);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!FuncionExists(funcion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");

            return View(funcion);
        }

        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .FirstOrDefault(x => x.Id == id);

            if (funcion == null)
            {
                return NotFound();
            }


            return View(funcion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult DeleteConfirmed(int id)
        {
            var funcion = _context.Funciones.Find(id);

            _context.Funciones.Remove(funcion);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult SeleccionarFiltro()
        {
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");

            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult FiltrarPorPelicula(Pelicula pelicula)
        {
            var funciones = _context
                 .Funciones
                 .Include(x => x.Pelicula)
                 .Include(x => x.Sala).ThenInclude(x => x.Tipo)
                 .Where(x => x.PeliculaId == pelicula.Id && x.Fecha >= DateTime.Now)
                 .ToList();

            if (!funciones.Any())
            {
                return RedirectToAction(nameof(NoHayFunciones));
            }

            return View("FuncionesFiltradas", funciones);
        }


        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult FiltroPelicula(int PeliculaId)
        {

            var funciones = _context
               .Funciones
               .Include(x => x.Pelicula)
               .Include(x => x.Sala).ThenInclude(x => x.Tipo)
               .Where(x => x.PeliculaId == PeliculaId && x.Fecha >= DateTime.Now)
               .ToList();

            if (!funciones.Any())
            {
                return RedirectToAction(nameof(NoHayFunciones));
            }

            return View("FuncionesFiltradas", funciones);

        }


        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult FiltroFecha(DateTime fecha)
        {
            var funciones =
                _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala).ThenInclude(x => x.Tipo)
                .Where(x => x.Fecha == fecha)
                .ToList();

            if (!funciones.Any())
            {
                return RedirectToAction(nameof(NoHayFunciones));
            }

            return View("FuncionesFiltradas", funciones);
        }


        [HttpGet]
        public IActionResult NoHayFunciones()
        {
            return View();
        }



        ////---- Métodos privados para validaciones ----////

        private bool FuncionExists(int id)
        {
            return _context.Funciones.Any(x => x.Id == id);
        }


        private void ValidarFecha(Funcion funcion)
        {
            if (funcion.Fecha < DateTime.Now)
            {
                ModelState.AddModelError(nameof(funcion.Fecha), "La fecha no puede ser anterior a la fecha actual");
            }

            if (funcion.Fecha.Year > DateTime.Now.Year + 1)
            {
                ModelState.AddModelError(nameof(funcion.Fecha), "La fecha debe ser dentro del año actual");
            }
        }


        private void ValidarHorario(Funcion funcion)
        {
            if (funcion.Horario.Hour > 1 && funcion.Horario.Hour < 9)
            {
                ModelState.AddModelError(nameof(funcion.Horario), "El horario debe estar comprendido entre las 9:00 y la 01:59 (A.M.)");
            }

            ValidarSalaLibre(funcion);

        }


        // Valida que la sala se encuentre libre cuando quiero crear o editar una funcion.
        private void ValidarSalaLibre(Funcion f)
        {

            if (_context.Funciones.Any(
                x => x.Fecha == f.Fecha &&
                x.SalaId == f.SalaId &&
               (x.Horario.Hour >= f.Horario.Hour - 3 && x.Horario.Hour <= f.Horario.Hour + 3) &&
                x.Id != f.Id))
            {
                ModelState.AddModelError(nameof(f.Horario), "La sala está ocupada en ese horario");
            }
        }

        // Valida que la cantidad de butacas total de la sala nueva no sea menor a la cantidad ya reservada para la función.
        private void ValidarCapacidadNuevaSalaSegunReservas(int funcionId, Funcion funcion)
        {
            var funcionDb = _context.Funciones.Find(funcionId);
            var salaAnterior = _context.Salas.Find(funcionDb.SalaId);
            var salaNueva = _context.Salas.Find(funcion.SalaId);

            var cantidadButacasReservadas = salaAnterior.CapacidadTotal - funcionDb.CantButacasDisponibles;
            funcionDb.CantButacasDisponibles = salaNueva.CapacidadTotal - cantidadButacasReservadas;

            if (funcionDb.CantButacasDisponibles < 0)
            {
                ModelState.AddModelError(nameof(funcion.SalaId), "No se puede cambiar la sala, dado que no es posible disminuir la cantidad de butacas por debajo de la cantidad ya reservada");
            }

        }


    }

}
