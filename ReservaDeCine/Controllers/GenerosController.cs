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
    public class GenerosController : Controller
    {
        private readonly CineDbContext _context;

        public GenerosController(CineDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Generos.ToList());
        }


        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = _context
                .Generos
                .Include(x => x.Peliculas).ThenInclude(x => x.Pelicula)
                .FirstOrDefault(m => m.Id == id);


            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Descripcion")] Genero genero)
        {
            ValidarDescripcionExistente(genero.Descripcion, genero.Id);

            if (ModelState.IsValid)
            {
                _context.Add(genero);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(genero);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = _context.Generos.Find(id);

            if (genero == null)
            {
                return NotFound();
            }
            return View(genero);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Descripcion")] Genero genero)
        {

            if (id != genero.Id)
            {
                return NotFound();
            }

            ValidarDescripcionExistente(genero.Descripcion, genero.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genero);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneroExists(genero.Id))
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
            return View(genero);
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = _context.Generos
                        .FirstOrDefault(x => x.Id == id);

            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var genero = _context.Generos.Find(id);

            _context.Generos.Remove(genero);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        ////---- Métodos privados para validaciones ----////

        private bool GeneroExists(int id)
        {
            return _context.Generos.Any(e => e.Id == id);
        }


        private void ValidarDescripcionExistente(string descripcion, int id)
        {
            if (_context.Generos.Any(e => Comparar(e.Descripcion, descripcion) && e.Id != id))
            {
                ModelState.AddModelError(nameof(Genero.Descripcion), "Ya existe ese género");
            }
        }


        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }
    }
}
