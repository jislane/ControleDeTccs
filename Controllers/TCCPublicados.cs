using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;
using SistemaDeControleDeTCCs.Models.ViewModels;
using SistemaDeControleDeTCCs.Services;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class TCCPublicados : Controller
    {
        private readonly SistemaDeControleDeTCCsContext _context;

        public TCCPublicados(SistemaDeControleDeTCCsContext context)
        {
            _context = context;
        }
        
        public IActionResult Index(string filterTema, string filterDiscente)
        {
            List<Tcc> tccs = _context.Tccs.ToList();
            List<Usuario> usuarios = new List<Usuario>();
            List<Banca> banca = new List<Banca>();
            foreach (Tcc item in tccs)
            {
                item.Usuario = _context.Usuario.Find(item.UsuarioId);
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
            TccViewModel viewModel = new TccViewModel { Tccs = tccs.OrderBy(x => x.Tema).ToList(), Banca = banca };

            return View(viewModel);

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

        public IActionResult Resumo(int? id)
        {
            return View(_context.Tccs.Find(id));
        }

        [HttpGet]
        public JsonResult GetDiscentes(int idCurso, String? nome)
        {

            if (nome != null)
            {
                return Json(_context
                    .Usuario
                    .Where(u => u.IdCurso == idCurso)
                    .Where(u => u.TipoUsuario.DescTipo == "Aluno" )
                    .Where(u => u.Nome.ToLower().Contains(nome.ToLower()))
                    .Select(u => new { Nome = u.Nome, Id = u.Id })
                    .Take(10).ToList());
            }
            else
            {
                return Json(_context
                    .Usuario
                    .Where(u => u.IdCurso == idCurso)
                    .Where(u => u.TipoUsuario.DescTipo == "Aluno" )                    
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
                    .Where(u => u.TipoUsuario.DescTipo == "Professor" || u.TipoUsuario.DescTipo == "Coordenador")
                    .Where(u => u.Nome.ToLower().Contains(nome.ToLower()))
                    .Select(u => new { Nome = u.Nome, Id = u.Id })
                    .Take(10).ToList());
            }
            else
            {
                return Json(_context
                    .Usuario
                    .Where(u => u.Curso.IdCampus == idCampus)
                    .Where(u => u.TipoUsuario.DescTipo == "Professor" || u.TipoUsuario.DescTipo == "Coordenador")
                    .Select(u => new { Nome = u.Nome, Id = u.Id })
                    .Take(10).ToList());
            }
            
        }

    }

}