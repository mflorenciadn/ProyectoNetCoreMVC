using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Grupo3.ReservaDeCine.Database;
using Grupo3.ReservaDeCine.Models;
using Microsoft.AspNetCore.Authorization;
using Grupo3.ReservaDeCine.Models.Enums;
using Grupo3.ReservaDeCine.Extensions;
using System.Text.RegularExpressions;

namespace Grupo3.ReservaDeCine.Controllers
{

    //[Authorize(Roles = nameof(Role.Administrador))]
    public class UsuariosController : Controller
    {
        private readonly CineDbContext _context;

        public UsuariosController(CineDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.SelectRoles = new SelectList(Enum.GetNames(typeof(Role)), "Id");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string password, Usuario usuario)
        {

            ValidarUserNameExistente(usuario.Username);
            ValidarPassword(password);

            if (ModelState.IsValid)
            {
                usuario.Password = password.Encriptar();
                _context.Add(usuario);
                await _context.SaveChangesAsync();
              
                return RedirectToAction("Index");
            }
         
            ViewBag.SelectRoles = new SelectList(Enum.GetNames(typeof(Role)), "Id");
            return View(usuario);
        }



        private void ValidarUserNameExistente(string username)
        {
            if (_context.Usuarios.Any(x => Comparar(x.Username, username)))
            {
                ModelState.AddModelError(nameof(username), "Nombre de usuario no disponible");
            }
        }


        //Función que compara que los nombres no sean iguales, ignorando espacios y case. 
        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }


        public void ValidarPassword (string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña es requerida.");
            }

            if (password.Length < 8)
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña debe tener al menos 8 caracteres.");
            }

            bool contieneUnNumero = new Regex("[0-9]").Match(password).Success;
            bool contieneUnaMinuscula = new Regex("[a-z]").Match(password).Success;
            bool contieneUnaMayuscula = new Regex("[A-Z]").Match(password).Success;

            if (!contieneUnNumero || !contieneUnaMinuscula || !contieneUnaMayuscula)
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña debe contener al menos un número, una minúscula y una mayúscula.");
            }
        }
    }

    
}