using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;
using SistemaDeControleDeTCCs.Models.ViewModels;
using SistemaDeControleDeTCCs.Services;

namespace SistemaDeControleDeTCCs.Controllers
{
    [Authorize(Roles = "Professor, Coordenador")]
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
                foreach (Banca b in _context.Banca.Where(x => x.TccId == item.TccId).ToList())
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
            else if (filterSemestre == -1)
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
        public IActionResult AddOrEdit(int id)
        {
          
            Tcc tcc = new Tcc();
            Usuario orientador = new Usuario();
           
            if (id != 0)
            {
                _context.Tccs.Include(t => t.Curso).Include(t => t.Usuario) ;
                tcc = _context.Tccs.Include(t => t.Curso).Include(t => t.Usuario)
                    .Where(t => t.TccId == id).First();
                Usuario orientadorAtual = _context.Banca
                    .Include(b => b.Usuario)
                    .Where(x => x.TccId == tcc.TccId && x.TipoUsuario.DescTipo.ToLower().Equals("orientador"))
                    .Select(x => x.Usuario).FirstOrDefault();
                ViewBag.orientadorAtual = orientadorAtual;
                ViewBag.campusSelected = _context.Campus.Find(tcc.Curso.IdCampus);
            }
            ViewBag.Campus = new SelectList(_context.Campus.ToList(), "Id", "Nome");
            TccViewModel viewModel = new TccViewModel {Tcc = tcc };
            return View(viewModel);
        }

       
        [Authorize(Roles = "Coordenador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string idOrientador, [Bind("TccId,IdCurso, Tema,UsuarioId,DataDeCadastro")] Tcc tcc)
        {

            if (ModelState.IsValid)
            {
                var c = _context.Cursos.Find(tcc.IdCurso);
                var dis = _context.Usuario.Where(u => u.Id == tcc.UsuarioId && u.IdCurso == tcc.IdCurso).ToList();
                var or = _context.Usuario.Where(u => u.Id == idOrientador && u.IdCurso == tcc.IdCurso).ToList();
                if (dis.Count == 0 || or.Count == 0) {
                    ModelState.AddModelError(string.Empty, "Os dados Informados são inválidos.");
                    if (tcc.TccId == 0)
                        return AddOrEdit(0);
                    return AddOrEdit(tcc.TccId);
                }

                if (tcc.TccId == 0)
                {
                    tcc.DataDeCadastro = DateTime.Now;
                    tcc.Status = _context.Status.Where(x => x.DescStatus.Contains("Cadastrado")).FirstOrDefault();
                    _context.Add(tcc);
                    await _context.SaveChangesAsync();
                    //Adiconarndo o orientador a Banca
                    Banca banca = new Banca();
                    banca.Tcc = tcc;
                    banca.TipoUsuario = _context
                        .TipoUsuario
                        .Where(x => x.DescTipo.ToLower().Equals("orientador"))
                        .Single();
                    banca.Usuario = _context.Usuario.Where(x => x.Id == idOrientador).Single();
                    banca.DataDeCadastro = DateTime.Now;
                    _context.Add(banca);

                    _context.LogAuditoria.Add(
                 new LogAuditoria
                 {
                     EmailUsuario = User.Identity.Name,
                     Ip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString(),
                     Date = DateTime.Now.ToLongDateString(),
                     DetalhesAuditoria = "Cadastrou o TCC",
                     IdItem = tcc.TccId

                 });

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
                    Banca banca = _context.Banca.Where(x => x.Tcc.TccId == tcc.TccId).First();
                    banca.Tcc = tcc;
                    banca.TipoUsuario = _context.TipoUsuario.Where(x => x.DescTipo.ToLower().Equals("orientador")).Single();
                    banca.Usuario = _context.Usuario.Where(x => x.Id == idOrientador).Single();
                    banca.DataDeCadastro = DateTime.Now;
                    _context.Update(banca);

                    _context.LogAuditoria.Add(
                 new LogAuditoria
                 {
                     EmailUsuario = User.Identity.Name,
                     Ip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString(),
                     Date = DateTime.Now.ToLongDateString(),
                     DetalhesAuditoria = "Atualizou o TCC",
                     IdItem = tcc.TccId

                 });

                    
                    await _context.SaveChangesAsync();
                    //*****************
                    

                }

                return RedirectToAction(nameof(Index));
            }
            return View(tcc);
        }

        [Authorize(Roles = "Professor,Coordenador")]
        [HttpGet]
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

            if ((tcc.Resumo == null || tcc.Resumo.Equals("")) && _context.FileTCC.Where(x => x.TccId == tcc.TccId).ToList().Count == 0)
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

        [Authorize(Roles = "Professor,Coordenador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDataLocalApresentacao(Tcc tccAtualizado, IList<int> checkNotificaMembrosBanca)
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
                TempData["Success"] = "Banca homologada com sucesso";
                if (checkNotificaMembrosBanca.Count > 0)
                {
                    tcc.Usuario = _context.Usuario.Where(x => x.Id == tccAtualizado.UsuarioId).FirstOrDefault();
                    List<Banca> banca = _context.Banca.Where(x => x.TccId == tcc.TccId).ToList();
                    FileTCC file = _context.FileTCC.Where(f => f.TccId == tcc.TccId).OrderByDescending(f => f.DataCadastro).FirstOrDefault();
                    foreach (Banca item in banca)
                    {
                        _senderEmail.NotificarMembrosBancaViaEmail(tcc, _context.Usuario.Where(x => x.Id == item.UsuarioId).FirstOrDefault(), file.Id.ToString());
                    }
                    _senderEmail.NotificarMembrosBancaViaEmail(tcc, tcc.Usuario, file.Id.ToString());
                    TempData["Success"] += " e enviada notificação via e-mail para os membros da banca";
                }
                TempData["Success"] += ".";
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

            _context.LogAuditoria.Add(
                 new LogAuditoria
                 {
                     EmailUsuario = User.Identity.Name,
                     Ip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString(),
                     Date = DateTime.Now.ToLongDateString(),
                     DetalhesAuditoria = "Removeu o TCC",
                     IdItem = tcc.TccId

                 });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Coordenador")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var tcc = await _context.Tccs.FindAsync(id);
            tcc.StatusId = _context.Status.Where(x => x.DescStatus.ToLower().Equals("cancelado")).Select(x => x.StatusId).FirstOrDefault();
            _context.Update(tcc);

            _context.LogAuditoria.Add(
                 new LogAuditoria
                 {
                     EmailUsuario = User.Identity.Name,
                     Ip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString(),
                     Date = DateTime.Now.ToLongDateString(),
                     DetalhesAuditoria = "Cancelou o TCC",
                     IdItem = tcc.TccId

                 });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Operação concluída! O TCC foi cancelado.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Resumo(int? id)
        {
            return View("Resumo", _context.Tccs.Where(t => t.TccId == id).ToList());
        }


        // JSONs


        [HttpGet]
        public JsonResult GetDiscentes(int idCurso, String? nome)
        {

            if (nome != null)
            {
                return Json(_context
                    .Usuario
                    .Where(u => u.IdCurso == idCurso)
                    .Where(u => u.TipoUsuario.DescTipo == "Aluno")
                    .Where(u => u.Nome.ToLower().Contains(nome.ToLower()))
                    .Select(u => new { Nome = u.Nome, Id = u.Id })
                    .Take(10).ToList());
            }
            else
            {
                return Json(_context
                    .Usuario
                    .Where(u => u.IdCurso == idCurso)
                    .Where(u => u.Nome.ToLower().Contains(nome.ToLower()))
                    .Select(u => new { Nome = u.Nome, Id = u.Id })
                    .Take(10).ToList());
            }


        }
        [HttpGet]
        public JsonResult GetOrientadores(int idCampus, String? nome)
        {
            if (nome != null)
            {
                return Json(_context
                    .Usuario
                    .Where(u => u.Curso.IdCampus == idCampus)
                    .Where(u => u.TipoUsuario.DescTipo == "Professor" 
                        || u.TipoUsuario.DescTipo == "Professor")
                    .Where(u => u.Nome.ToLower().Contains(nome.ToLower()))
                    .Select(u => new { Nome = u.Nome, Id = u.Id })
                    .Take(10).ToList());
            }
            else
            {
                return Json(_context
                    .Usuario
                    .Where(u => u.Curso.IdCampus == idCampus)
                    .Where(u => u.TipoUsuario.DescTipo == "Professor"
                        || u.TipoUsuario.DescTipo == "Professor")
                    .Select(u => new { Nome = u.Nome, Id = u.Id })
                    .Take(10).ToList());
            }
            
        }

        public JsonResult GetCursos(int idCampus, String? nome)
        {
            if (nome != null)
            {
                return Json(_context
                    .Cursos
                    .Where(c => c.IdCampus == idCampus)
                    .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                    .Select(c => new
                    {
                        Nome = c.Nome,
                        Id = c.Id
                    })
                    .Take(10).ToList());
            }
            else {
                return Json(_context
                    .Cursos
                    .Where(c => c.IdCampus == idCampus)
                    .Select(c => new {
                        Nome = c.Nome,
                        Id = c.Id
                    })
                    .Take(10).ToList());
            }
            
        }
    }

}