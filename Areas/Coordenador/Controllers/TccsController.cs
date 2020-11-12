using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;
using SistemaDeControleDeTCCs.Models.ViewModels;
using SistemaDeControleDeTCCs.Services;

namespace SistemaDeControleDeTCCs.Controllers
{
    [Area("Coordenador")]
    [Authorize(Policy = "Coordenador")]
    public class TccsController : Controller
    {
        private readonly SistemaDeControleDeTCCsContext _context;
        private readonly SenderEmail _senderEmail;

        public TccsController(SistemaDeControleDeTCCsContext context, SenderEmail senderEmail)
        {
            _context = context;
            _senderEmail = senderEmail;
        }

        // GET: Tccs
        public IActionResult Index(string filterTema, string filterDiscente, int filterStatus, int filterSemestre)
        {
            List<Tcc> tccs = _context.Tccs.ToList();
            List<Usuario> usuarios = new List<Usuario>();
            List<Banca> banca = new List<Banca>();
            foreach (Tcc item in tccs)
            {
                item.Usuario = _context.Usuario.Find(item.UsuarioId);
                item.Status = _context.Status.Find(item.StatusId);
                if (!usuarios.Contains(item.Usuario))
                    usuarios.Add(item.Usuario);
                List<Banca> membrosBanca = _context.Banca.Where(x => x.TccId == item.TccId).ToList();
                foreach(Banca b in _context.Banca.Where(x => x.TccId == item.TccId).ToList())
                {
                    b.Usuario = _context.Usuario.Find(b.UsuarioId);
                    b.TipoUsuario = _context.TipoUsuario.Find(b.TipoUsuarioId);
                    banca.Add(b);
                }
            }
            // filtros
            if (!string.IsNullOrEmpty(filterTema))
            {
                tccs = tccs.Where(x => x.Tema.ToUpper().Contains(filterTema.ToUpper())).ToList();
                ViewData["filterTema"] = filterTema;
            }
            if (!string.IsNullOrEmpty(filterDiscente))
            {
                tccs = tccs.Where(x => x.UsuarioId == filterDiscente).ToList();
                ViewBag.Discente = new SelectList(usuarios, "Id", "Nome", filterDiscente);
            }
            else
            {
                ViewBag.Discente = new SelectList(usuarios, "Id", "Nome");
            }
            if (filterStatus > 0)
            {
                tccs = tccs.Where(x => x.StatusId == filterStatus).ToList();
                ViewBag.Status = new SelectList(_context.Status.ToList(), "StatusId", "DescStatus", filterStatus);
            }
            else
            {
                ViewBag.Status = new SelectList(_context.Status.ToList(), "StatusId", "DescStatus");
            }
            if (filterSemestre > 0)
            {
                Calendario calendario = _context.Calendario.Where(x => x.CalendarioId == filterSemestre).FirstOrDefault();
                tccs = tccs.Where(x => x.DataApresentacao >= calendario.DataInicio && x.DataApresentacao <= calendario.DataFim).ToList();
                var calendarios = _context.Calendario.Select(x => new { Value = x.CalendarioId, Text = string.Format("{0}.{1}", x.Ano, x.Semestre) }).ToList();
                calendarios.Add(new { Value = -1, Text = "Sem data" });
                ViewBag.Semestre = new SelectList(calendarios.OrderByDescending(x => x.Text), "Value", "Text", filterSemestre);
            }
            else if(filterSemestre == -1)
            {
                tccs = tccs.Where(x => x.DataApresentacao == null).ToList();
                var calendarios = _context.Calendario.Select(x => new { Value = x.CalendarioId, Text = string.Format("{0}.{1}", x.Ano, x.Semestre) }).ToList();
                calendarios.Add(new { Value = -1, Text = "Sem data" });
                ViewBag.Semestre = new SelectList(calendarios.OrderByDescending(x => x.Text), "Value", "Text", filterSemestre);
            }
            else
            {
                var calendarios = _context.Calendario.Select(x => new { Value = x.CalendarioId, Text = string.Format("{0}.{1}", x.Ano, x.Semestre) }).ToList();
                calendarios.Add(new { Value = -1, Text = "Sem data" });
                ViewBag.Semestre = new SelectList(calendarios.OrderByDescending(x => x.Text), "Value", "Text");
            }

            TccViewModel viewModel = new TccViewModel { Tccs = tccs.OrderBy(x => x.Tema).ToList(), Banca = banca };
            return View(viewModel);
        }

        // GET: Tccs/Create
        public IActionResult AddOrEdit(int id = 0)
        {                           
            List<Usuario> discentes = _context.Usuario.Where(x => x.TipoUsuario.DescTipo.Contains("Aluno")).ToList();
            Tcc tcc = new Tcc();
            Usuario orientador = new Usuario();
            if (id != 0)
            {
                tcc = _context.Tccs.Find(id);
                string orientadorId = _context.Banca.Where(x => x.TccId == tcc.TccId && x.TipoUsuario.DescTipo.ToLower().Equals("orientador")).Select(x => x.UsuarioId).FirstOrDefault();
                ViewBag.ProfessorList = new SelectList(_context.Usuario.Where(x => x.TipoUsuarioId.Equals(5) || x.TipoUsuarioId.Equals(1)).OrderBy(x => x.Nome), "Id", "Nome", orientadorId);
            }
            else
            {
                ViewBag.ProfessorList = new SelectList(_context.Usuario.Where(x => x.TipoUsuarioId.Equals(5) || x.TipoUsuarioId.Equals(1)).OrderBy(x => x.Nome), "Id", "Nome");
            }
            TccViewModel viewModel = new TccViewModel { Usuarios = discentes, Tcc = tcc };
            return View(viewModel);
        }

        // POST: Tccs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string OrientadorId, [Bind("TccId,Tema,UsuarioId,DataDeCadastro")] Tcc tcc)
        {

            if (ModelState.IsValid)
            {
                if (tcc.TccId == 0)
                {
                    tcc.DataDeCadastro = DateTime.Now;
                    tcc.Status = _context.Status.Where(x => x.DescStatus.Contains("Pendente")).FirstOrDefault();
                    _context.Add(tcc);
                    await _context.SaveChangesAsync();
                    //Adiconarndo o orientador a Banca
                    Banca banca = new Banca();
                    banca.Tcc = tcc;
                    banca.TipoUsuario = _context.TipoUsuario.Where(x => x.TipoUsuarioId == 7).Single();
                    banca.Usuario = _context.Usuario.Where(x => x.Id == OrientadorId).Single();
                    banca.DataDeCadastro = DateTime.Now;
                    _context.Add(banca);
                    await _context.SaveChangesAsync();
                    //*****************
                    var discente = _context.Usuario.Where(x => x.Id == tcc.UsuarioId).FirstOrDefault();
                    //_senderEmail.NotificarDiscenteCadastroTCCViaEmail(discente, tcc.Tema);
                }
                else
                {
                    tcc.StatusId = _context.Tccs.Where(x => x.TccId == tcc.TccId).Select(x => x.StatusId).FirstOrDefault();
                    _context.Update(tcc);
                    await _context.SaveChangesAsync();
                    //Adiconarndo o orientador a Banca
                    Banca banca = _context.Banca.Where(x => x.Tcc.TccId == tcc.TccId && x.TipoUsuario.TipoUsuarioId == 7).Single();
                    banca.Tcc = tcc;
                    banca.TipoUsuario = _context.TipoUsuario.Where(x => x.TipoUsuarioId == 7).Single();
                    banca.Usuario = _context.Usuario.Where(x => x.Id == OrientadorId).Single();
                    banca.DataDeCadastro = DateTime.Now;
                    _context.Update(banca);
                    await _context.SaveChangesAsync();
                    //*****************

                }

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
