using Microsoft.AspNetCore.Mvc;
using MVC.ReservaDeCine.Database;
using MVC.ReservaDeCine.Extensions;
using MVC.ReservaDeCine.Models;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MVC.ReservaDeCine.Controllers
{
    public class AdministradoresController : Controller
    {
        private readonly CineDbContext _context;

        public AdministradoresController(CineDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            var administrador = _context.Administradores.FirstOrDefault(x => x.Id == id);

            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Legajo, Nombre, Apellido, Email, Username")] Administrador administrador, string password)
        {

            ValidarPassword(password);


            if (ModelState.IsValid)
            {
                administrador.Password = password.Encriptar();
                _context.Add(administrador);
                _context.SaveChanges();

                return RedirectToAction("Ingresar", "Cuentas");
            }

            return View(administrador);
        }



        ////---- Métodos privados para validaciones ----////

        public void ValidarPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(nameof(Administrador.Password), "La contraseña es requerida");
            }

            if (password.Length < 8)
            {
                ModelState.AddModelError(nameof(Administrador.Password), "La contraseña debe tener al menos 8 caracteres");
            }

            bool contieneUnNumero = new Regex("[0-9]").Match(password).Success;
            bool contieneUnaMinuscula = new Regex("[a-z]").Match(password).Success;
            bool contieneUnaMayuscula = new Regex("[A-Z]").Match(password).Success;

            if (!contieneUnNumero || !contieneUnaMinuscula || !contieneUnaMayuscula)
            {
                ModelState.AddModelError(nameof(Administrador.Password), "La contraseña debe contener al menos un número, una minúscula y una mayúscula");
            }
        }

    }
}
