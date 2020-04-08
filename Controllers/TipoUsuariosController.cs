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
    public class TipoUsuariosController : Controller
    {
        private readonly ContextoGeral _context;

        public TipoUsuariosController(ContextoGeral context)
        {
            _context = context;
        }

        // GET: TipoUsuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoUsuario.ToListAsync());
        }

        // GET: TipoUsuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoUsuario = await _context.TipoUsuario
                .FirstOrDefaultAsync(m => m.TipoUsuarioId == id);
            if (tipoUsuario == null)
            {
                return NotFound();
            }

            return View(tipoUsuario);
        }

        // GET: TipoUsuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoUsuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoUsuarioId,DescTipo")] TipoUsuario tipoUsuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoUsuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoUsuario);
        }

        // GET: TipoUsuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoUsuario = await _context.TipoUsuario.FindAsync(id);
            if (tipoUsuario == null)
            {
                return NotFound();
            }
            return View(tipoUsuario);
        }

        // POST: TipoUsuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TipoUsuarioId,DescTipo")] TipoUsuario tipoUsuario)
        {
            if (id != tipoUsuario.TipoUsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoUsuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoUsuarioExists(tipoUsuario.TipoUsuarioId))
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
            return View(tipoUsuario);
        }

        // GET: TipoUsuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoUsuario = await _context.TipoUsuario
                .FirstOrDefaultAsync(m => m.TipoUsuarioId == id);
            if (tipoUsuario == null)
            {
                return NotFound();
            }

            return View(tipoUsuario);
        }

        // POST: TipoUsuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoUsuario = await _context.TipoUsuario.FindAsync(id);
            _context.TipoUsuario.Remove(tipoUsuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoUsuarioExists(int id)
        {
            return _context.TipoUsuario.Any(e => e.TipoUsuarioId == id);
        }
    }
}
