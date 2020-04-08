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
    public class TccsController : Controller
    {
        private readonly ContextoGeral _context;

        public TccsController(ContextoGeral context)
        {
            _context = context;
        }

        // GET: Tccs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tccs.ToListAsync());
        }

        // GET: Tccs/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Tcc());
            else
                return View(_context.Tccs.Find(id));
        }

        // POST: Tccs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TccId,Tema,DataDeCadastro,cpf")] Tcc tcc)
        {
            if (ModelState.IsValid)
            {
                if (tcc.TccId == 0)
                    _context.Add(tcc);
                else
                    _context.Update(tcc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tcc);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var tcc = await _context.Tccs.FindAsync(id);
            _context.Tccs.Remove(tcc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
