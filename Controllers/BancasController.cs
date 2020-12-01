using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class BancasController : Controller
    {

        private IHostingEnvironment _appEnvironment;

        private readonly SistemaDeControleDeTCCsContext _context;

        public BancasController(SistemaDeControleDeTCCsContext context, IHostingEnvironment env)
        {
            _context = context;
            _appEnvironment = env;
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
            else if (User.IsInRole("Professor"))
            {
                string userId = _context.Users.FirstOrDefault(p => p.UserName == User.Identity.Name).Id;
                result = await _context.Banca.Where(x => x.TipoUsuarioId == 7 && x.UsuarioId == userId).OrderByDescending(x => x.DataDeCadastro).ToListAsync();
            }
            else if (User.IsInRole("Aluno"))
            {
                string userId = _context.Users.FirstOrDefault(p => p.UserName == User.Identity.Name).Id;
                var tccList = await _context.Tccs.Where(t => t.UsuarioId == userId).ToListAsync();
                foreach (var item in tccList)
                {
                    result.Add(await _context.Banca.Where(b => b.TccId == item.TccId && b.TipoUsuarioId == 7).OrderByDescending(x => x.DataDeCadastro).FirstAsync());
                }
            }

            foreach (var item in result)
            {
                item.Tcc = _context.Tccs.Find(item.TccId);
                item.Tcc.Usuario = _context.Usuario.Find(item.Tcc.UsuarioId);
                item.Usuario = _context.Usuario.Find(item.UsuarioId);
                item.TipoUsuario = _context.TipoUsuario.Find(item.TipoUsuarioId);
            }

            if (User.IsInRole("Aluno"))
            {
                return View("indexAluno", result);
            }

            return View(result);

        }

        // GET: Bancas/Details/5

        public async Task<IActionResult> Details(int? id, double? Nota_1, double? Nota_2, double? Nota_3, double? Nota_4, double? Nota_5, string? DataApresentacao, string? LocalApresentacao)
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

            AtualizarNotas(Nota_1, Nota_2, Nota_3, Nota_4, Nota_5, banca.TccId);


            List<Banca> resultTemp = _context.Banca.Where(b => b.TccId == banca.TccId).OrderBy(b => b.BancaId).ToList();
            List<Banca> result = new List<Banca>();



            foreach (var item in resultTemp)
            {
                item.Usuario = _context.Usuario.Find(item.UsuarioId);
                item.TipoUsuario = _context.TipoUsuario.Find(item.TipoUsuarioId);
                result.Add(item);
            }

            ViewBag.menbrosBanca = result;

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

        public async Task<IActionResult> ResumoEdit(int TccId, List<IFormFile> arquivos, [Bind("Resumo")] Tcc tcc)
        {
            if (TccId == 0)
            {
                return NotFound();
            }
            var result = _context.Tccs.Where(t => t.TccId == TccId).FirstOrDefault();
            result.Resumo = tcc.Resumo;
            _context.Update(result);
            await _context.SaveChangesAsync();

            List<FileTCC> filesTcc = _context.FileTCC.Where(x => x.TccId == TccId).ToList();
            result.Status = _context.Status.Where(x => x.StatusId == result.StatusId).FirstOrDefault();
            if (result.Status.DescStatus.ToLower().Equals("cadastrado")
                && result.Resumo != null && !result.Resumo.Equals("")
                && filesTcc != null && filesTcc.Count > 0)
            {
                result.StatusId = _context.Status.Where(x => x.DescStatus.ToLower().Contains("pré-homologado")).Select(x => x.StatusId).FirstOrDefault();
                _context.Update(result);
                await _context.SaveChangesAsync();
            }

            if (arquivos.Count != 0)
            {
                SaveFile(arquivos, TccId);
            }

            return View("Resumo", result);
        }
        public IActionResult Resumo(int? id)
        {
            return View(_context.Tccs.Find(id));
        }


        public async Task<IActionResult> Anexo(int? id)
        {
            ViewBag.TccId = id;
            List<FileTCC> result = await _context.FileTCC.Where(f => f.TccId == id).OrderByDescending(f => f.DataCadastro).ToListAsync();

            foreach (var item in result)
            {
                item.Tcc = _context.Tccs.Find(item.TccId);
                item.Tcc.Usuario = _context.Usuario.Find(item.Tcc.UsuarioId);
            }

            return View("Anexo",result);
        }


        public async Task<IActionResult> SaveFile(List<IFormFile> arquivos, int TccId)
        {

            var arquivo = arquivos.First();
            var fileBinary = new byte[arquivo.Length];
            using (var stream = arquivo.OpenReadStream())
            {
                stream.Read(fileBinary, 0, fileBinary.Length); 
            }
                
            var fileSample = new FileTCC
            {
                Id = Guid.NewGuid(),
                Name = arquivo.FileName,
                Extension = arquivo.ContentType,//"pdf",
                DataCadastro = DateTime.Now,
                Length = arquivo.Length,
                FileStream = fileBinary,
                TccId = TccId
            };
            _context.Add(fileSample);
            await _context.SaveChangesAsync();

            Tcc tcc = _context.Tccs.Where(x => x.TccId == TccId).FirstOrDefault();
            tcc.Status = _context.Status.Where(x => x.StatusId == tcc.StatusId).FirstOrDefault();
            List<FileTCC> filesTcc = _context.FileTCC.Where(x => x.TccId == TccId).ToList();
            if (tcc.Status.DescStatus.ToLower().Equals("cadastrado")
                && tcc.Resumo != null && !tcc.Resumo.Equals("")
                && filesTcc != null && filesTcc.Count > 0)
            {
                tcc.StatusId = _context.Status.Where(x => x.DescStatus.ToLower().Contains("pré-homologado")).Select(x => x.StatusId).FirstOrDefault();
                _context.Update(tcc);
                await _context.SaveChangesAsync();
            }

            //return RedirectToAction("Index");
            ViewBag.TccId = TccId;
            return View("Anexo", _context.FileTCC.Where(f => f.TccId == TccId));
        }

        private bool BancaExists(int id)
        {
            return _context.Banca.Any(e => e.BancaId == id);
        }

        private void AtualizarNotas(double? Nota_1, double? Nota_2, double? Nota_3, double? Nota_4, double? Nota_5, int TccId)
        {
            var count = 1;
            double notas = 0;
            if (Nota_1 != null)
            {
                var resultTemp = _context.Banca.Where(b => b.TccId == TccId).OrderBy(b => b.BancaId).ToList();
                foreach (var item in resultTemp)
                {
                    if (count == 1)
                    {
                        notas += Nota_1.Value;
                        item.Nota = Nota_1;
                        _context.Update(item);
                        _context.SaveChanges();
                    }
                    if (count == 2)
                    {
                        notas += Nota_2.Value;
                        item.Nota = Nota_2;
                        _context.Update(item);
                        _context.SaveChanges();
                    }
                    if (count == 3)
                    {
                        notas += Nota_3.Value;
                        item.Nota = Nota_3;
                        _context.Update(item);
                        _context.SaveChanges();
                    }
                    if (count == 4)
                    {
                        notas += Nota_4.Value;
                        item.Nota = Nota_4;
                        _context.Update(item);
                        _context.SaveChanges();
                    }
                    if (count == 5)
                    {
                        notas += Nota_5.Value;
                        item.Nota = Nota_5;
                        _context.Update(item);
                        _context.SaveChanges();
                    }
                    count++;
                }
                var result = _context.Tccs.Where(t => t.TccId == TccId).First();
                result.Nota = notas / resultTemp.Count;
                result.StatusId = result.Nota >= 6 ? _context.Status.Where(x => x.DescStatus.ToLower().Equals("aprovado")).Select(x => x.StatusId).FirstOrDefault() : _context.Status.Where(x => x.DescStatus.ToLower().Equals("reprovado")).Select(x => x.StatusId).FirstOrDefault();
                _context.Update(result);
                _context.SaveChanges();
            }
        }
    }
}
