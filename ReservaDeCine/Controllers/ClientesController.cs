using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.ReservaDeCine.Database;
using MVC.ReservaDeCine.Extensions;
using MVC.ReservaDeCine.Models;
using MVC.ReservaDeCine.Models.Enums;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MVC.ReservaDeCine.Controllers
{

    public class ClientesController : Controller
    {
        private readonly CineDbContext _context;

        public ClientesController(CineDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Index()
        {
            return View(_context.Clientes.ToList());
        }


        [HttpGet]
        [Authorize]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            var cliente = _context.Clientes
                            .Include(x => x.Reservas).ThenInclude(x => x.Funcion).ThenInclude(x => x.Pelicula)
                            .FirstOrDefault(x => x.Id == id);


            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Registrar([Bind("Nombre, Apellido, FechaDeNacimiento, Email, Username")] Cliente cliente, string password)
        {

            ComprobarFechaDeNacimiento(cliente.FechaDeNacimiento);
            ValidarEmailExistente(cliente.Email, cliente.Id);
            ValidarUserNameExistente(cliente.Username);
            ValidarPassword(password);

            if (ModelState.IsValid)
            {
                cliente.FechaDeAlta = DateTime.Now;
                cliente.Password = password.Encriptar();
                _context.Add(cliente);
                _context.SaveChanges();

                return RedirectToAction(nameof(CreateExitoso));
            }

            return View(cliente);
        }

        public IActionResult CreateExitoso()
        {
            return View();
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Edit(int id, Cliente cliente, string password)
        {
            return EditarCliente(id, cliente, password);
        }

        [HttpGet]
        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult EditMe()
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id);
            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult EditMe([Bind("Id, Nombre, Apellido, FechaDeNacimiento, Email")] Cliente cliente, string password)
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id);

            return EditarCliente(id, cliente, password);
        }


        private IActionResult EditarCliente(int id, Cliente cliente, string password)
        {
            if (cliente.Id != id)
            {
                return NotFound();
            }

            ComprobarFechaDeNacimiento(cliente.FechaDeNacimiento);
            ModelState.Remove(nameof(Cliente.Username));

            if (!string.IsNullOrWhiteSpace(password))
            {
                ValidarPassword(password);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Cliente clienteDb = _context.Clientes.Find(cliente.Id);

                    clienteDb.Email = cliente.Email;
                    clienteDb.FechaDeNacimiento = cliente.FechaDeNacimiento;
                    clienteDb.Nombre = cliente.Nombre;
                    clienteDb.Apellido = cliente.Apellido;

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        clienteDb.Password = password.Encriptar();
                    }

                    _context.Update(clienteDb);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (User.IsInRole(nameof(Role.Administrador)))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View(cliente);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = _context.Clientes
                .FirstOrDefault(x => x.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult DeleteConfirmed(int id)
        {
            var cliente = _context.Clientes.Find(id);

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        ////---- Métodos privados para validaciones ----////

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(x => x.Id == id);
        }


        //Edad mínima para crear un usuario: 12 años, Máxima: 100 años.
        private void ComprobarFechaDeNacimiento(DateTime fechaNacimiento)
        {
            var edad = fechaNacimiento.CalcularEdad();

            if (edad < 12 && edad > 0)
            {
                ModelState.AddModelError(nameof(Cliente.FechaDeNacimiento), "El cliente debe ser mayor de 12 años");
            }

            if ((edad > 100) || edad <= 0)
            {
                ModelState.AddModelError(nameof(Cliente.FechaDeNacimiento), "Fecha de nacimiento inválida");
            }
        }


        private void ValidarUserNameExistente(string username)
        {
            if (_context.Usuarios.Any(x => Comparar(x.Username, username)))
            {
                ModelState.AddModelError(nameof(Cliente.Username), "Nombre de usuario no disponible");
            }
        }


        private void ValidarEmailExistente(string mail, int id)
        {
            if (_context.Clientes.Any(x => Comparar(x.Email, mail) && x.Id != id))
            {
                ModelState.AddModelError(nameof(Cliente.Email), "Ya existe un cliente con este Email");
            }
        }


        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }


        public void ValidarPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña es requerida");
            }

            if (password.Length < 8)
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña debe tener al menos 8 caracteres");
            }

            bool contieneUnNumero = new Regex("[0-9]").Match(password).Success;
            bool contieneUnaMinuscula = new Regex("[a-z]").Match(password).Success;
            bool contieneUnaMayuscula = new Regex("[A-Z]").Match(password).Success;

            if (!contieneUnNumero || !contieneUnaMinuscula || !contieneUnaMayuscula)
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña debe contener al menos un número, una minúscula y una mayúscula");
            }
        }

    }

}
