using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.ReservaDeCine.Database;
using MVC.ReservaDeCine.Models;
using MVC.ReservaDeCine.Models.Enums;
using System;
using System.Linq;

namespace MVC.ReservaDeCine.Controllers
{
    [Authorize(Roles = nameof(Role.Administrador))]
    public class ClasificacionesController : Controller
    {
        private readonly CineDbContext _context;

        public ClasificacionesController(CineDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Clasificaciones.ToList());
        }


        [HttpGet]
        public IActionResult Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var clasificacion = _context.Clasificaciones
                .Include(x => x.Peliculas)
                .FirstOrDefault(x => x.Id == id);

            if (clasificacion == null)
            {
                return NotFound();
            }

            return View(clasificacion);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Descripcion,EdadMinima")] Clasificacion clasificacion)
        {

            ValidarDescripcionExistente(clasificacion);

            if (ModelState.IsValid)
            {
                _context.Add(clasificacion);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(clasificacion);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificacion = _context.Clasificaciones.Find(id);

            if (clasificacion == null)
            {
                return NotFound();
            }
            return View(clasificacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Descripcion,EdadMinima")] Clasificacion clasificacion)
        {
            if (id != clasificacion.Id)
            {
                return NotFound();
            }

            ValidarDescripcionExistente(clasificacion);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clasificacion);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClasificacionExists(clasificacion.Id))
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
            return View(clasificacion);
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificacion = _context.Clasificaciones
                .FirstOrDefault(x => x.Id == id);

            if (clasificacion == null)
            {
                return NotFound();
            }

            return View(clasificacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var clasificacion = _context.Clasificaciones.Find(id);
            _context.Clasificaciones.Remove(clasificacion);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        ////---- Métodos privados para validaciones ----////

        private bool ClasificacionExists(int id)
        {
            return _context.Clasificaciones.Any(x => x.Id == id);
        }


        private void ValidarDescripcionExistente(Clasificacion clasificacion)
        {
            if (_context.Clasificaciones.Any(x => Comparar(x.Descripcion, clasificacion.Descripcion) && x.Id != clasificacion.Id))
            {
                ModelState.AddModelError(nameof(clasificacion.Descripcion), "Ya existe una clasificación con este nombre");
            }
        }


        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }
    }
}
