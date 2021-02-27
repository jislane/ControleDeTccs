using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    [Authorize(Policy = "Administrador")]
    public class CursosController : Controller

    {
        private readonly SistemaDeControleDeTCCsContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CursosController(SistemaDeControleDeTCCsContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: Administrador/Cursos
        public async Task<IActionResult> Index()
        {
            var sistemaDeControleDeTCCsContext = _context.Cursos.Include(c => c.Campus).Include(c => c.Coordenador);
            return View(await sistemaDeControleDeTCCsContext.ToListAsync());
        }

        // GET: Administrador/Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include(c => c.Campus)
                .Include(c => c.Coordenador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // GET: Administrador/Cursos/Create
        public IActionResult Create()
        {
            ViewData["IdCampus"] = new SelectList(_context.Campus, "Id", "Endereco");
            return View();
        }

        // POST: Administrador/Cursos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Sigla,CodEmec,IdCampus")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);

					_context.LogAuditoria.Add(
                   new LogAuditoria
                   {
                       EmailUsuario = User.Identity.Name,
                       DetalhesAuditoria = string.Concat("Cadastrou o curso de Id:",
                  curso.Id, "Data de cadastro: ", DateTime.Now.ToLongDateString())
                   });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCampus"] = new SelectList(_context.Campus, "Id", "Endereco", curso.IdCampus);
            return View(curso);
        }

        // GET: Administrador/Cursos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }
            var coordenadores = new ArrayList();
            coordenadores.Add(new Usuario
            {
                Id = null,
                Nome = "Não Definido"
            });
            coordenadores.AddRange(
           _context.Usuario.
               Where(
               u => (u.TipoUsuario.DescTipo.Equals("Professor") || u.TipoUsuario.DescTipo.Equals("Coordenador"))
               && u.IdCurso == curso.Id).ToList());
            ViewData["IdCampus"] = new SelectList(_context.Campus, "Id", "Nome", curso.IdCampus);
            ViewData["Coordenadores"] = new SelectList(coordenadores, "Id", "Nome", curso.IdCoordenador);
            TempData["idCoordenadorAtual"] = curso.IdCoordenador;
            return View(curso);
            /*ViewData["IdCampus"] = new SelectList(_context.Campus, "Id", "Endereco", curso.IdCampus);
            ViewData["IdCoordenador"] = new SelectList(_context.Usuario, "Id", "Id", curso.IdCoordenador);
            return View(curso);*/
        }

        // POST: Administrador/Cursos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Sigla,CodEmec,IdCoordenador,IdCampus")] Curso curso)
        {
            if (id != curso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tipoProf = _context.TipoUsuario.Where(t => t.DescTipo.Equals("Professor")).First();
                    var tipoCoord = _context.TipoUsuario.Where(t => t.DescTipo.Equals("Coordenador")).First();
                    //var roleProf = _roleManager.FindByNameAsync(tipoProf.DescTipo).Result;
                    //var roleCoord = _roleManager.FindByNameAsync(tipoCoord.DescTipo).Result;

                    if (TempData["idCoordenadorAtual"].ToString() != null && curso.IdCoordenador != TempData["idCoordenadorAtual"].ToString())
                    {
                        Usuario coordenadorAtual = await _context.Usuario.FindAsync(TempData["idCoordenadorAtual"].ToString());
                        coordenadorAtual.TipoUsuario = tipoProf;

                        await _userManager.RemoveFromRoleAsync(coordenadorAtual, tipoCoord.DescTipo);
                        // Adiciona a Role Nova
                        await _userManager.AddToRoleAsync(coordenadorAtual, tipoProf.DescTipo);
                    }
                    if (curso.IdCoordenador != null && curso.IdCoordenador != TempData["idCoordenadorAtual"].ToString())
                    {
                        Usuario novoCoordenador = await _context.Usuario.FindAsync(curso.IdCoordenador);
                        novoCoordenador.TipoUsuario = tipoCoord;
                        _context.Update(novoCoordenador);

                        await _userManager.RemoveFromRoleAsync(novoCoordenador, tipoProf.DescTipo);
                        // Adiciona a Role Nova
                        await _userManager.AddToRoleAsync(novoCoordenador, tipoCoord.DescTipo);
                    }
                    TempData.Remove("idCoordenadorAtual");
                    _context.Update(curso);
					_context.LogAuditoria.Add(
                   new LogAuditoria
                   {
                       EmailUsuario = User.Identity.Name,
                       DetalhesAuditoria = string.Concat("Atualizou o curso de Id:",
                  curso.Id, "Data da atualização: ", DateTime.Now.ToLongDateString())
                   });
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.Id))
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

            var coordenadores = new ArrayList();
            coordenadores.Add(new Usuario
            {
                Id = null,
                Nome = "Não Definido"
            });
            coordenadores.AddRange(
            _context.Usuario.
              Where(
              u => (u.TipoUsuario.DescTipo.Equals("Professor") || u.TipoUsuario.DescTipo.Equals("Coordenador"))
              && u.IdCurso == curso.Id).ToList());


            ViewData["IdCampus"] = new SelectList(_context.Campus, "Id", "Nome", curso.IdCampus);
            ViewData["Coordenadores"] = new SelectList(coordenadores, "Id", "Nome", curso.IdCoordenador);
            return View(curso);
        }

        // GET: Administrador/Cursos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include(c => c.Campus)
                .Include(c => c.Coordenador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // POST: Administrador/Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            _context.Cursos.Remove(curso);

			_context.LogAuditoria.Add(
                   new LogAuditoria
                   {
                       EmailUsuario = User.Identity.Name,
                       DetalhesAuditoria = string.Concat("Removeu o curso de Id:",
                  curso.Id, "Data da exclusão: ", DateTime.Now.ToLongDateString())
                   });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursoExists(int id)
        {
            return _context.Cursos.Any(e => e.Id == id);
        }


    }
}
