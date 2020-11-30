using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Controllers;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Controllers
{
    [Area("Coordenador")]
    [Authorize(Roles = "Coordenador")]
    public class RelatoriosController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SistemaDeControleDeTCCsContext _context;

        public RelatoriosController(SistemaDeControleDeTCCsContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var calendarios = _context.Calendario.Select(x => new { Value = x.CalendarioId, Text = string.Format("{0}.{1}", x.Ano, x.Semestre) }).ToList();
            //calendarios.Add(new { Value = -1, Text = "Sem data" });
            ViewBag.Semestre = new SelectList(calendarios.OrderByDescending(x => x.Text), "Value", "Text");
            return View();
        }

        private RelatorioAtividades getRelatorio()
        {
            var rpt = new RelatorioAtividades();
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            rpt.BasePath = contentRootPath;

            rpt.PageTitle = "SISTEMA DE CONTROLE DE TCC";
            rpt.PageSubTitle = "Coordenação do Bacharelado de Sistemas de Informação - CBSI";
            rpt.ImprimirCabecalhoPadrao = true;
            rpt.ImprimirRodapePadrao = true;

            return rpt;
        }

        public ActionResult Preview(int filterSemestre)
        {
            RelatorioAtividades rpt = getRelatorio();
            string semestre = "";
            List<Tcc> tccs = null;
            List<Banca> bancas = null;

            if (filterSemestre > 0)
            {
                tccs = _context.Tccs.Where(x => x.StatusId == 3).ToList();
                bancas = new List<Banca>();
                foreach (Tcc tcc in tccs)
                {
                    tcc.Usuario = _context.Usuario.Where(x => x.Id.Equals(tcc.UsuarioId)).FirstOrDefault();
                    Banca banca = _context.Banca.Where(x => x.TccId == tcc.TccId && x.TipoUsuarioId == 7).FirstOrDefault();
                    banca.Usuario = _context.Usuario.Where(x => x.Id.Equals(banca.UsuarioId)).FirstOrDefault();
                    bancas.Add(banca);
                }
                Calendario calendario = _context.Calendario.Where(x => x.CalendarioId == filterSemestre).FirstOrDefault();
                if(calendario != null)
                    semestre = calendario.Ano + "." + calendario.Semestre;
                tccs = tccs.Where(x => x.DataApresentacao >= calendario.DataInicio && x.DataApresentacao <= calendario.DataFim).ToList();
                var calendarios = _context.Calendario.Select(x => new { Value = x.CalendarioId, Text = string.Format("{0}.{1}", x.Ano, x.Semestre) }).ToList();
                //calendarios.Add(new { Value = -1, Text = "Sem data" });
                ViewBag.Semestre = new SelectList(calendarios.OrderByDescending(x => x.Text), "Value", "Text", filterSemestre);
            }
            else if (filterSemestre == -1)
            {
                tccs = tccs.Where(x => x.DataApresentacao == null).ToList();
                var calendarios = _context.Calendario.Select(x => new { Value = x.CalendarioId, Text = string.Format("{0}.{1}", x.Ano, x.Semestre) }).ToList();
                //calendarios.Add(new { Value = -1, Text = "Sem data" });
                ViewBag.Semestre = new SelectList(calendarios.OrderByDescending(x => x.Text), "Value", "Text", filterSemestre);
            }
            else
            {
                var calendarios = _context.Calendario.Select(x => new { Value = x.CalendarioId, Text = string.Format("{0}.{1}", x.Ano, x.Semestre) }).ToList();
                //calendarios.Add(new { Value = -1, Text = "Sem data" });
                ViewBag.Semestre = new SelectList(calendarios.OrderByDescending(x => x.Text), "Value", "Text");
            }

            return File(rpt.GetOutput(tccs, bancas, semestre).GetBuffer(), "application/pdf");
        }
    }
}