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
    [Authorize(Roles = "Professor")]
    public class TccsController : Controller
    {
        private readonly SistemaDeControleDeTCCsContext _context;
        private readonly SenderEmail _senderEmail;

        public TccsController(SistemaDeControleDeTCCsContext context, SenderEmail senderEmail)
        {
            _context = context;
            _senderEmail = senderEmail;
        }

        [Authorize(Roles = "Coordenador")]
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

        [Authorize(Roles = "Coordenador")]
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

        [Authorize(Roles = "Coordenador")]
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
                    _senderEmail.NotificarDiscenteCadastroTCCViaEmail(discente, tcc.Tema);
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

        [Authorize(Roles = "Professor")]
        public IActionResult AddDataLocalApresentacao(int id = 0)
        {
            Tcc tcc = new Tcc();
            if (id != 0)
            {
                tcc = _context.Tccs.Find(id);
                tcc.Usuario = _context.Usuario.Where(x => x.Id == tcc.UsuarioId).FirstOrDefault();
                Banca bancaOrientador = _context.Banca.Where(x => x.TccId == tcc.TccId && x.TipoUsuario.DescTipo.ToLower().Equals("orientador")).FirstOrDefault();
                ViewBag.Orientador = _context.Usuario.Where(x => x.Id == bancaOrientador.UsuarioId).FirstOrDefault();
                ViewBag.CalendarioAtivo = _context.Calendario.Where(x => x.Ativo == true).FirstOrDefault();
            }
            
            if ((tcc.Resumo == null || tcc.Resumo.Equals("") ) && _context.FileTCC.Where(x => x.TccId == tcc.TccId).ToList().Count == 0)
            {
                TempData["Error"] = "Homologação cancelada! Favor, solicite ao discente que insira o resumo e o arquivo do TCC.";
                return RedirectToAction("Index", "Bancas", new { area = "" });
            }
            else if (tcc.Resumo == null || tcc.Resumo.Equals(""))
            {
                TempData["Error"] = "Homologação cancelada! Favor, solicite ao discente que insira o resumo do TCC.";
                return RedirectToAction("Index", "Bancas", new { area = "" });
            }
            else if (_context.FileTCC.Where(x => x.TccId == tcc.TccId).ToList().Count == 0)
            {
                TempData["Error"] = "Homologação cancelada! Favor, solicite ao discente que insira o arquivo do TCC.";
                return RedirectToAction("Index", "Bancas", new { area = "" });
            }
            else if (_context.Banca.Where(x => x.TccId == tcc.TccId).ToList().Count < 3)
            {
                TempData["Error"] = "Homologação cancelada! Favor, adicione os membros da banca.";
                return RedirectToAction("Index", "Bancas", new { area = "" });
            }
            return View(tcc);
        }

        [Authorize(Roles = "Professor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDataLocalApresentacao(Tcc tccAtualizado)
        {
            Tcc tcc = _context.Tccs.Where(x => x.TccId == tccAtualizado.TccId).FirstOrDefault();
            Calendario calendarioAtivo = _context.Calendario.Where(x => x.Ativo == true).FirstOrDefault();
            if (tccAtualizado.DataApresentacao == null)
            {
                TempData["Error"] = "Favor, informe a data de apresentação.";
            }
            else if (tccAtualizado.DataApresentacao.Value.Date < calendarioAtivo.DataInicio.Date || tccAtualizado.DataApresentacao.Value.Date > calendarioAtivo.DataFim.Date)
            {
                TempData["Error"] = "Data de apresentação inválida! Favor, informe uma data entre " + calendarioAtivo.DataInicio.ToString("dd/MM/yyyy") + " à " + calendarioAtivo.DataFim.ToString("dd/MM/yyyy") + ".";
            }
            else if (tccAtualizado.LocalApresentacao == null)
            {
                TempData["Error"] = "Favor, informe o local da apresentação.";
            }
            else if (ModelState.IsValid)
            {
                tcc.DataApresentacao = tccAtualizado.DataApresentacao;
                tcc.LocalApresentacao = tccAtualizado.LocalApresentacao;
                tcc.StatusId = _context.Status.Where(x => x.DescStatus.Contains("Homologado Banca")).Select(x => x.StatusId).FirstOrDefault();
                _context.Update(tcc);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Bancas", new { area = "" });
            }

            if (tccAtualizado.TccId != 0)
            {
                tcc.Usuario = _context.Usuario.Where(x => x.Id == tccAtualizado.UsuarioId).FirstOrDefault();
                Banca bancaOrientador = _context.Banca.Where(x => x.TccId == tccAtualizado.TccId && x.TipoUsuario.DescTipo.ToLower().Equals("orientador")).FirstOrDefault();
                ViewBag.Orientador = _context.Usuario.Where(x => x.Id == bancaOrientador.UsuarioId).FirstOrDefault();
                ViewBag.CalendarioAtivo = _context.Calendario.Where(x => x.Ativo == true).FirstOrDefault();
            }
            
            return View(tcc);
        }

        [Authorize(Roles = "Coordenador")]
        public async Task<IActionResult> Delete(int? id)
        {
            var tcc = await _context.Tccs.FindAsync(id);
            _context.Tccs.Remove(tcc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Coordenador")]
        public async Task<IActionResult> Cancelar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tcc = await _context.Tccs.FindAsync(id);
            if (tcc == null)
            {
                return NotFound();
            }
            tcc.Usuario = _context.Usuario.Where(x => x.Id == tcc.UsuarioId).FirstOrDefault();
            return PartialView(tcc);
        }

        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarConfirmed(int id)
        {
            var tcc = await _context.Tccs.FindAsync(id);
            tcc.StatusId = _context.Status.Where(x => x.DescStatus.ToLower().Equals("cancelado")).Select(x => x.StatusId).FirstOrDefault();
            _context.Update(tcc);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Operação concluída! O TCC foi cancelado.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Resumo(int? id)
        {
            return View("Resumo", _context.Tccs.Where(t => t.TccId == id).ToList());
        }
    }
}
