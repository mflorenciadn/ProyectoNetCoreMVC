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
    [Authorize(Roles = nameof(Role.Administrador))]
    public class SalasController : Controller
    {

        private readonly CineDbContext _context;

        public SalasController(CineDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Salas.Include(x => x.Tipo).ToList());
        }


        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = _context.Salas
                .Include(x => x.Tipo)
                .Include(x => x.Funciones).ThenInclude(x => x.Pelicula)
                .FirstOrDefault(m => m.Id == id);


            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SelectTiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nombre,TipoId,CapacidadTotal")] Sala sala)
        {

            ValidarNombreExistente(sala);

            if (ModelState.IsValid)
            {
                _context.Add(sala);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SelectTiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");

            return View(sala);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = _context
                .Salas
                .FirstOrDefault(x => x.Id == id);

            if (sala == null)
            {
                return NotFound();
            }

            ViewBag.SelectTiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View(sala);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Sala sala)
        {
            if (id != sala.Id)
            {
                return NotFound();
            }

            ValidarNombreExistente(sala);
            ValidarCapacidadSegunReservas(id, sala.CapacidadTotal);

            if (ModelState.IsValid)
            {
                try
                {
                    AjustarDisponibilidadDeButacasEnFunciones(id, sala.CapacidadTotal);

                    _context.Update(sala);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaExists(sala.Id))
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

            ViewBag.SelectTiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View(sala);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = _context
                .Salas
                .Include(x => x.Tipo)
                .FirstOrDefault(m => m.Id == id);


            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var sala = _context.Salas
                .Include(x => x.Tipo)
                .Include(x => x.Funciones).ThenInclude(x => x.Pelicula)
                .FirstOrDefault(x => x.Id == id);

            _context.Salas.Remove(sala);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }






        ////---- Métodos privados para validaciones ----////

        private bool SalaExists(int id)
        {
            return _context.Salas.Any(e => e.Id == id);
        }


        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }


        private void ValidarNombreExistente(Sala sala)
        {
            if (_context.Salas.Any(e => Comparar(e.Nombre, sala.Nombre) && e.Id != sala.Id))
            {
                ModelState.AddModelError(nameof(sala.Nombre), "Ya existe una sala con ese nombre");
            }
        }

        //Se valida que al querer modificar la capacidad de una sala, no existan reservas con mayor cantidad de butacas
        private void ValidarCapacidadSegunReservas(int salaId, int capacidadSalaModificada)
        {
            var Funciones =
                _context.Funciones
                .Include(x => x.Reservas)
                .Where(x => x.Fecha > DateTime.Today && x.SalaId == salaId)
                .ToList();

            foreach (Funcion funcion in Funciones)
            {
                int sumaButacasReservadas = 0;

                foreach (Reserva reserva in funcion.Reservas)
                {
                    sumaButacasReservadas += reserva.CantButacas;
                }

                if (sumaButacasReservadas > capacidadSalaModificada)
                {
                    ModelState.AddModelError(nameof(Sala.CapacidadTotal), "No se puede disminuir la cantidad de butacas por debajo de la cantidad ya reserva");
                    return;
                }
            }
        }


        // Se usa al cambiar la capacidad de la sala, ajustando la disponibilidad en las funciones
        private void AjustarDisponibilidadDeButacasEnFunciones(int salaId, int capacidadTotal)
        {
            var Funciones =
                _context.Funciones
                .Include(x => x.Reservas)
                .Where(x => x.Fecha > DateTime.Today && x.SalaId == salaId)
                .ToList();

            foreach (Funcion funcion in Funciones)
            {
                int sumaButacasReservadas = 0;

                foreach (Reserva reserva in funcion.Reservas)
                {
                    sumaButacasReservadas += reserva.CantButacas;
                }

                funcion.CantButacasDisponibles = capacidadTotal - sumaButacasReservadas;
            }
        }

    }

}
