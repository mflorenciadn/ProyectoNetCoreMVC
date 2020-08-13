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
    public class TiposSalasController : Controller
    {
        private readonly CineDbContext _context;

        public TiposSalasController(CineDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.TiposSala.ToList());
        }


        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = _context.TiposSala
                .FirstOrDefault(m => m.Id == id);

            if (tipoSala == null)
            {
                return NotFound();
            }

            return View(tipoSala);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nombre,PrecioEntrada")] TipoSala tipoSala)
        {

            ValidarNombreExistente(tipoSala);

            if (ModelState.IsValid)
            {
                _context.Add(tipoSala);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(tipoSala);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = _context.TiposSala.Find(id);

            if (tipoSala == null)
            {
                return NotFound();
            }
            return View(tipoSala);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nombre,PrecioEntrada")] TipoSala tipoSala)
        {
            if (id != tipoSala.Id)
            {
                return NotFound();
            }

            ValidarNombreExistente(tipoSala);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoSala);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoSalaExists(tipoSala.Id))
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
            return View(tipoSala);
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = _context.TiposSala
                .FirstOrDefault(m => m.Id == id);

            if (tipoSala == null)
            {
                return NotFound();
            }

            return View(tipoSala);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tipoSala = _context.TiposSala.Find(id);
            _context.TiposSala.Remove(tipoSala);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        ////---- Métodos privados para validaciones ----////


        private bool TipoSalaExists(int id)
        {
            return _context.TiposSala.Any(x => x.Id == id);
        }


        private void ValidarNombreExistente(TipoSala tipoSala)
        {
            if (_context.TiposSala.Any(x => Comparar(x.Nombre, tipoSala.Nombre) && x.Id != tipoSala.Id))
            {
                ModelState.AddModelError(nameof(tipoSala.Nombre), "Ya existe un tipo de sala con ese nombre");
            }
        }


        private bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }
    }
}
