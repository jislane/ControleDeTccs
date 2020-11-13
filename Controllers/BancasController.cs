using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class BancasController : Controller
    {
        private readonly SistemaDeControleDeTCCsContext _context;

        public BancasController(SistemaDeControleDeTCCsContext context)
        {
            _context = context;
        }

        // GET: Bancas
        public async Task<IActionResult> Index()
        {
            //var sistemaDeControleDeTCCsContext = _context.Banca.Include(b => b.Tcc).Include(b => b.TipoUsuario).Include(b => b.Usuario);
            //return View(await sistemaDeControleDeTCCsContext.ToListAsync());
            List<Banca> result = new List<Banca>();
            if (User.IsInRole("Coordenador"))
            {
                result = await _context.Banca.Where(x => x.TipoUsuarioId == 7).OrderByDescending(x => x.DataDeCadastro).ToListAsync();
            }
            else
            {
                string userId = _context.Users.FirstOrDefault(p => p.UserName == User.Identity.Name).Id;
                result = await _context.Banca.Where(x => x.TipoUsuarioId == 7 && x.UsuarioId == userId).OrderByDescending(x => x.DataDeCadastro).ToListAsync();
            }

            foreach (var item in result)
            {
                item.Tcc = _context.Tccs.Find(item.TccId);
                item.Tcc.Usuario = _context.Usuario.Find(item.Tcc.UsuarioId);
                item.Usuario = _context.Usuario.Find(item.UsuarioId);
                item.TipoUsuario = _context.TipoUsuario.Find(item.TipoUsuarioId);
            }
            return View(result);

        }

        // GET: Bancas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banca = await _context.Banca
                .Include(b => b.Tcc)
                .Include(b => b.TipoUsuario)
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(m => m.BancaId == id);
            banca.Tcc.Usuario = _context.Usuario.Find(banca.Tcc.UsuarioId);

            List<Banca> resultTemp = _context.Banca.Where(b => b.TccId == banca.TccId && b.TipoUsuarioId != 7).ToList();
            List<Banca> result = new List<Banca>();
            foreach (var item in resultTemp)
            {
                item.Usuario = _context.Usuario.Find(item.UsuarioId);
                item.TipoUsuario = _context.TipoUsuario.Find(item.TipoUsuarioId);
                result.Add(item);
            }

            ViewData["menbrosBanca"] = result;

            if (banca == null)
            {
                return NotFound();
            }

            return View(banca);
        }

        // GET: Bancas/Create
        public IActionResult Create(int id)
        {
            ViewData["TccId"] = id;
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuario.Where(x => x.TipoUsuarioId == 5 || x.TipoUsuarioId == 3 || x.TipoUsuarioId == 1 || x.TipoUsuarioId == 2).OrderBy(x => x.DescTipo), "TipoUsuarioId", "DescTipo");
            ViewData["UsuarioId"] = new SelectList(_context.Usuario.Where(x => x.TipoUsuarioId == 5 || x.TipoUsuarioId == 3 || x.TipoUsuarioId == 1 || x.TipoUsuarioId == 2).OrderBy(x => x.Nome), "Id", "Nome");
            return View();
        }

        // POST: Bancas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int TccId, [Bind("UsuarioId,TipoUsuarioId")] Banca banca)
        {
            banca.DataDeCadastro = DateTime.Now;
            banca.Tcc = _context.Tccs.Find(TccId);
            banca.TipoUsuario = _context.TipoUsuario.Find(banca.TipoUsuarioId);
            banca.Usuario = _context.Usuario.Find(banca.UsuarioId);

            _context.Add(banca);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Bancas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banca = await _context.Banca.FindAsync(id);
            if (banca == null)
            {
                return NotFound();
            }
            ViewData["TccId"] = new SelectList(_context.Tccs, "TccId", "Tema", banca.TccId);
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuario, "TipoUsuarioId", "TipoUsuarioId", banca.TipoUsuarioId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "Id", banca.UsuarioId);
            return View(banca);
        }

        // POST: Bancas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BancaId,DataDeCadastro,TccId,UsuarioId,TipoUsuarioId,Nota")] Banca banca)
        {
            if (id != banca.BancaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(banca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BancaExists(banca.BancaId))
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
            ViewData["TccId"] = new SelectList(_context.Tccs, "TccId", "Tema", banca.TccId);
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuario, "TipoUsuarioId", "TipoUsuarioId", banca.TipoUsuarioId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "Id", banca.UsuarioId);
            return View(banca);
        }

        // GET: Bancas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banca = await _context.Banca
                .Include(b => b.Tcc)
                .Include(b => b.TipoUsuario)
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(m => m.BancaId == id);
            if (banca == null)
            {
                return NotFound();
            }

            return View(banca);
        }

        // POST: Bancas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banca = await _context.Banca.FindAsync(id);
            _context.Banca.Remove(banca);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BancaExists(int id)
        {
            return _context.Banca.Any(e => e.BancaId == id);
        }
    }
}
