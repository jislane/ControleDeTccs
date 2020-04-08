using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ContextoGeral _context;

        public UsuariosController(ContextoGeral context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToListAsync());
        }

        // GET: Usuarios/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Usuario());
            else
                return View(_context.Usuario.Find(id));
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("UsuarioId,Nome,Matricula,cpf,telefone,email")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.UsuarioId == 0) 
                _context.Add(usuario);
                else
                    _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }       
    }
}
