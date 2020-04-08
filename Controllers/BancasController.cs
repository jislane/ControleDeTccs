using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class BancasController : Controller
    {
        private readonly ContextoGeral _context;

        public BancasController(ContextoGeral context)
        {
            _context = context;
        }

        // GET: Bancas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Banca.ToListAsync());
        }

        // GET: Bancas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banca = await _context.Banca
                .FirstOrDefaultAsync(m => m.BancaId == id);
            if (banca == null)
            {
                return NotFound();
            }

            return View(banca);
        }

        // GET: Bancas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bancas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BancaId,DataDeCadastro")] Banca banca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(banca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(banca);
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
            return View(banca);
        }

        // POST: Bancas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BancaId,DataDeCadastro")] Banca banca)
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
