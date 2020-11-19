using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class FileTCCsController : Controller
    {
        private readonly SistemaDeControleDeTCCsContext _context;
        private IHostingEnvironment _appEnvironment;
        public FileTCCsController(SistemaDeControleDeTCCsContext context, IHostingEnvironment env)
        {
            _context = context;
            _appEnvironment = env;
        }

        // GET: FileTCCs
        public async Task<IActionResult> Index()
        {
            var sistemaDeControleDeTCCsContext = _context.FileTCC.Include(f => f.Tcc);
            return View(await sistemaDeControleDeTCCsContext.ToListAsync());
        }

        // GET: FileTCCs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileTCC = await _context.FileTCC
                .Include(f => f.Tcc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileTCC == null)
            {
                return NotFound();
            }

            return View(fileTCC);
        }

        // GET: FileTCCs/Create
        public IActionResult Create()
        {
            ViewData["TccId"] = new SelectList(_context.Tccs, "TccId", "Tema");
            return View();
        }

        // POST: FileTCCs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Extension,DataCadastro,Length,FileStream,TccId")] FileTCC fileTCC)
        {
            if (ModelState.IsValid)
            {
                fileTCC.Id = Guid.NewGuid();
                _context.Add(fileTCC);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TccId"] = new SelectList(_context.Tccs, "TccId", "Tema", fileTCC.TccId);
            return View(fileTCC);
        }

        // GET: FileTCCs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileTCC = await _context.FileTCC.FindAsync(id);
            if (fileTCC == null)
            {
                return NotFound();
            }
            ViewData["TccId"] = new SelectList(_context.Tccs, "TccId", "Tema", fileTCC.TccId);
            return View(fileTCC);
        }

        // POST: FileTCCs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Extension,DataCadastro,Length,FileStream,TccId")] FileTCC fileTCC)
        {
            if (id != fileTCC.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fileTCC);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileTCCExists(fileTCC.Id))
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
            ViewData["TccId"] = new SelectList(_context.Tccs, "TccId", "Tema", fileTCC.TccId);
            return View(fileTCC);
        }

        // GET: FileTCCs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileTCC = await _context.FileTCC
                .Include(f => f.Tcc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileTCC == null)
            {
                return NotFound();
            }

            return View(fileTCC);
        }

        // POST: FileTCCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fileTCC = await _context.FileTCC.FindAsync(id);
            _context.FileTCC.Remove(fileTCC);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Bancas");
        }


        public async Task<IActionResult> Download(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var result = await _context.FileTCC.FindAsync(id);
            return File(result.FileStream, "application/pdf");
        }



        private bool FileTCCExists(Guid id)
        {
            return _context.FileTCC.Any(e => e.Id == id);
        }
    }
}
