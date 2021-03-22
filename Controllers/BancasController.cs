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
using SistemaDeControleDeTCCs.Models.ViewModels;
using SistemaDeControleDeTCCs.Utils;

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
                result = await _context.Banca.Where(x => x.TipoUsuario.DescTipo.ToLower().Equals("orientador"))
                    .OrderByDescending(x => x.DataDeCadastro).ToListAsync();
            }
            else if (User.IsInRole("Professor"))
            {
                string userId = _context.Users.FirstOrDefault(p => p.UserName == User.Identity.Name).Id;
                result = await _context.Banca.Where(x => x.TipoUsuario.DescTipo.ToLower().Equals("orientador")
                        && x.UsuarioId == userId)
                    .OrderByDescending(x => x.DataDeCadastro).ToListAsync();
            }
            else if (User.IsInRole("Aluno"))
            {
                string userId = _context.Users.FirstOrDefault(p => p.UserName == User.Identity.Name).Id;
                var tccList = await _context.Tccs.Where(t => t.UsuarioId == userId).ToListAsync();
                foreach (var item in tccList)
                {
                    result.Add(await _context.Banca.Where(b => b.TccId == item.TccId
                            && b.TipoUsuario.DescTipo.ToLower().Equals("orientador"))
                        .OrderByDescending(x => x.DataDeCadastro).FirstAsync());
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

        // o id é o id do Tcc
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            Tcc tcc = _context.Tccs.Find(id);

            BancaViewModel bancaViewModel = new BancaViewModel();

                bancaViewModel.Membros = new List<Banca>();
                bancaViewModel.Membros.Add(_context.Banca
                    .Include(b => b.Tcc)
                    .Include(b => b.TipoUsuario)
                    .Include(b => b.Usuario)
                    .Where(b => b.TccId == tcc.TccId)
                    .Where(b => b.TipoUsuario.DescTipo.ToLower().Equals("orientador"))
                    .Select(b => new Banca {
                        BancaId = b.BancaId,
                        Tcc = b.Tcc,
                        DataDeCadastro = b.DataDeCadastro,
                        Nota = b.Nota,
                        TipoUsuario = b.TipoUsuario,
                        TccId = b.TccId,
                        TipoUsuarioId = b.TipoUsuarioId,
                        UsuarioId = b.UsuarioId,
                        Usuario = new Usuario {
                            Id = b.UsuarioId,
                            Nome = b.Usuario.Nome
                        }
                    })
                    .First()
                );

                bancaViewModel.Membros.AddRange(_context.Banca
                    .Include(b => b.TipoUsuario)
                    .Include(b => b.Usuario)
                    .Where(b => b.TccId == tcc.TccId)
                    .Where(b => !b.TipoUsuario.DescTipo.ToLower().Equals("orientador"))
                    .Select(b => new Banca
                    {
                        Nota = b.Nota,
                        TipoUsuario = b.TipoUsuario,
                        UsuarioId = b.UsuarioId,
                        BancaId = b.BancaId,
                        Usuario = new Usuario
                        {
                            Id = b.UsuarioId,
                            Nome = b.Usuario.Nome
                        }
                    })
                .ToList());
         
            if (bancaViewModel.Membros == null) {
                return NotFound();
            }
            bancaViewModel.BancaId = id.Value;

                return View(bancaViewModel);
        }

        [HttpPost]
        // o id é o id do Tcc
        public JsonResult Details(int id, [FromBody] List<NotaMembroBanca> notas)
        {
           // [FromBody] System.Text.Json.JsonElement entity
            if (id == 0 || notas == null)
            {
                return new JsonResult("Faltando Dados")
                {
                    StatusCode = 400
                };

            }
            bool isValid = isvalidNotas(notas);
            if (!isValid) {
                return new JsonResult("Notas incorretas, favor conferir!")
                {
                    StatusCode = 400
                };
            }
            var notaTcc = AtualizarNotas(id, notas);
            return new JsonResult(new {notaTcc = notaTcc })
            {
                StatusCode = 200
                
            };

        }

        /* public async Task<IActionResult> Details(int? id, double? Nota_1, double? Nota_2, double? Nota_3, double? Nota_4, double? Nota_5, string? DataApresentacao, string? LocalApresentacao, List<SistemaDeControleDeTCCs.Utils.NotaMembroBanca>? notasMembros)
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
        */

        // GET: Bancas/Create
        public IActionResult Create(int id)
        {
            ViewData["TccId"] = id;
            ViewData["TipoUsuarioId"] = new 
                SelectList(_context.TipoUsuario
                .Where(x => !x.DescTipo.ToLower().Equals("administrador")
                    && !x.DescTipo.ToLower().Equals("orientador")
                    && !x.DescTipo.ToLower().Equals("aluno")
                    )
                .OrderBy(x => x.DescTipo), "TipoUsuarioId", "DescTipo");
            ViewData["UsuarioId"] = new SelectList(_context
                .Usuario
                .Where(u => u.Curso.IdCampus == _context.Tccs.Where(t => t.TccId == id).Include(t => t.Curso).First().Curso.IdCampus)
                .Where(x => !x.TipoUsuario.DescTipo.ToLower().Equals("administrador")
                    && !x.TipoUsuario.DescTipo.ToLower().Equals("orientador")
                    && !x.TipoUsuario.DescTipo.ToLower().Equals("aluno")
                )
                .OrderBy(x => x.Nome), "Id", "Nome");
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

            _context.LogAuditoria.Add(
                    new LogAuditoria
                    {
                        EmailUsuario = User.Identity.Name,
                        DetalhesAuditoria = string.Concat("Cadastrou a banca de Id:",
                   banca.BancaId, "Data de cadastro: ", DateTime.Now.ToLongDateString())
                    });
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

                    _context.LogAuditoria.Add(
                    new LogAuditoria
                    {
                        EmailUsuario = User.Identity.Name,
                        DetalhesAuditoria = string.Concat("Atualizou a banca de Id:",
                   banca.BancaId, "Data da atualização: ", DateTime.Now.ToLongDateString())
                    });
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

            _context.LogAuditoria.Add(
                    new LogAuditoria
                    {
                        EmailUsuario = User.Identity.Name,
                        DetalhesAuditoria = string.Concat("Removeu a banca de Id:",
                   banca.BancaId, "Data da exclusão: ", DateTime.Now.ToLongDateString())
                    });
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
                        if (Nota_1 != null) {
                            notas += Nota_1.Value;
                            item.Nota = Nota_1;
                            _context.Update(item);
                            _context.SaveChanges();
                        }
                           
                       
                    }
                    if (count == 2)
                    {
                        if (Nota_2 != null) 
                        {
                            notas += Nota_2.Value;
                            item.Nota = Nota_2;
                            _context.Update(item);
                            _context.SaveChanges();
                        }
                        
                    }
                    if (count == 3)
                    {
                        if (Nota_3 != null)
                        {
                            notas += Nota_3.Value;
                            item.Nota = Nota_3;
                            _context.Update(item);
                            _context.SaveChanges();
                        }
                           
                        
                    }
                    if (count == 4)
                    {
                        if (Nota_4 != null) {
                            notas += Nota_4.Value;
                            item.Nota = Nota_4;
                            _context.Update(item);
                            _context.SaveChanges();
                        }
                        
                    }
                    if (count == 5)
                    {
                        if (Nota_5 != null) {
                            notas += Nota_5.Value;
                            item.Nota = Nota_5;
                            _context.Update(item);
                            _context.SaveChanges();
                        }
                            
                       
                    }
                    count++;
                }
                var result = _context.Tccs.Where(t => t.TccId == TccId).First();
                result.Nota = notas / resultTemp.Count;
                result.StatusId = result.Nota >= 6 ? _context.Status.Where(x => x.DescStatus.ToLower().Equals("aprovado")).Select(x => x.StatusId).FirstOrDefault() : _context.Status.Where(x => x.DescStatus.ToLower().Equals("reprovado")).Select(x => x.StatusId).FirstOrDefault();
                _context.Update(result);

                _context.LogAuditoria.Add(
                    new LogAuditoria
                    {
                        EmailUsuario = User.Identity.Name,
                        DetalhesAuditoria = string.Concat("Atualizou as notas:",
                   notas, "Data de cadastro: ", DateTime.Now.ToLongDateString())
                    });
                _context.SaveChanges();
            }
        }

        private  double AtualizarNotas(int tccId, List<NotaMembroBanca> notas)
        {
            var resultTemp = _context.Banca.Where(b => b.TccId == tccId);
            var soma = 0.0;
            var tccID = 0;
            foreach (var b in notas)
            {
                var c = resultTemp.Where(u => u.UsuarioId.Equals("cc856c71-b6f1-407d-bf0f-001c9315d67c"));

                var banca = resultTemp.Where(u => u.UsuarioId.Equals(b.MembroBanca)).First();
                if (banca != null) {
                    if (b.Nota == null) 
                    {
                        soma += 0;
                    }
                    else
                    {
                        soma += b.Nota.Value;
                    }
                    if (tccID == 0)
                        tccID = banca.TccId;
                    banca.Nota = b.Nota;
                    _context.Update(banca);
                }

            }
            var result = _context.Tccs.Where(t => t.TccId == tccID).First();
            result.Nota = soma / resultTemp.Count();
            _context.SaveChanges();
            return soma / resultTemp.Count();
        }


        private bool isvalidNotas(List<NotaMembroBanca> notas)
        {
            foreach (var nM in notas)
            {
                if (nM.Nota == null) {
                    continue;
                }
                if (nM.Nota < 0 || nM.Nota > 10)
                    return false;
            }
            return true;
        }
    }
}
