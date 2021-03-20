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

namespace SistemaDeControleDeTCCs.Controllers
{
    [Authorize(Policy = "Coordenador")]
    public class CalendariosController : Controller
    {
        private readonly SistemaDeControleDeTCCsContext _context;

        public CalendariosController(SistemaDeControleDeTCCsContext context)
        {
            _context = context;
        }

        // GET: Calendarios
        public async Task<IActionResult> Index()
        {
            _context.LogAuditoria.Add(
               new LogAuditoria
               {
                   EmailUsuario = User.Identity.Name,
                   DetalhesAuditoria = "Entrou na tela de listagem de calendarios"
               });
            return View(await _context.Calendario.OrderByDescending(x => x.Ativo).ThenByDescending(x => x.Ano).ThenByDescending(x => x.Semestre).ToListAsync());
        }

        // GET: Calendarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var calendario = await _context.Calendario
                .FirstOrDefaultAsync(m => m.CalendarioId == id);
            if (calendario == null)
            {
                return NotFound();
            }

           

            return View(calendario);
        }

        // GET: Calendarios/Create
        public IActionResult Create()
        {
            _context.LogAuditoria.Add(
               new LogAuditoria
               {
                   EmailUsuario = User.Identity.Name,
                   DetalhesAuditoria = "Entrou na tela de cadastro de calendarios"
               });

            _context.SaveChanges();


            return View();
        }

        // POST: Calendarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CalendarioId,Ano,Semestre,DataInicio,DataFim,Ativo")] Calendario calendario)
        {
            if (ModelState.IsValid)
            {
                if (_context.Calendario.Where(x => x.Ano == calendario.Ano && x.Semestre == calendario.Semestre).ToList().Count > 0)
                {
                    TempData["Error"] = "Operação cancelada! Já existe um Calendário de banca cadastrado para o ano " + calendario.Ano + "." + calendario.Semestre;
                    return View(calendario);
                }
                else if (calendario.Ativo && _context.Calendario.Where(x => x.Ativo == true).ToList().Count > 0)
                {
                    TempData["Error"] = "Operação cancelada! Não é possível ter mais de um calendário de banca ativo simultaneamente.";
                    return View(calendario);
                }

                _context.LogAuditoria.Add(
                new LogAuditoria
                {
                    EmailUsuario = User.Identity.Name,
                    DetalhesAuditoria = string.Concat("Cadastrou um calendario:",
                    calendario.CalendarioId, "Data de cadastro: ", DateTime.Now.ToLongDateString())
                });

                _context.Add(calendario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Calendário de banca cadastrado com sucesso.";
                return RedirectToAction(nameof(Index));

                

                _context.SaveChanges();
            }
            return View(calendario);
        }

        // GET: Calendarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendario = await _context.Calendario.FindAsync(id);
            if (calendario == null)


            {
                return NotFound();
            }

            _context.LogAuditoria.Add(
                new LogAuditoria
                {
                    EmailUsuario = User.Identity.Name,
                    DetalhesAuditoria = "Entrou na tela de edição de calendarios:"

                });


            return View(calendario);
        }

        // POST: Calendarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CalendarioId,Ano,Semestre,DataInicio,DataFim,Ativo")] Calendario calendario)
        {
            if (id != calendario.CalendarioId)
            {
                return NotFound();
            }

            _context.LogAuditoria.Add(
               new LogAuditoria
               {
                   EmailUsuario = User.Identity.Name,
                   DetalhesAuditoria = string.Concat("Editou o calendario:",
                   calendario.CalendarioId, "Data da edição: ", DateTime.Now.ToLongDateString())
               });

            if (ModelState.IsValid)
            {
                if (_context.Calendario.Where(x => x.Ano == calendario.Ano && x.Semestre == calendario.Semestre && x.CalendarioId != calendario.CalendarioId).ToList().Count > 0)
                {
                    TempData["Error"] = "Operação cancelada! Já existe um Calendário de banca cadastrado para o ano " + calendario.Ano + "." + calendario.Semestre;
                    return View(calendario);
                }
                else if (calendario.Ativo && _context.Calendario.Where(x => x.Ativo == true && x.CalendarioId != calendario.CalendarioId).ToList().Count > 0)
                {
                    TempData["Error"] = "Operação cancelada! Não é possível ter mais de um calendário de banca ativo simultaneamente.";
                    return View(calendario);
                }

                try
                {
                    _context.Update(calendario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalendarioExists(calendario.CalendarioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               

                TempData["Success"] = "Calendário de banca alterado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            return View(calendario);
        }

        //GET: Calendarios/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var calendario = await _context.Calendario.FindAsync(id);
            _context.Calendario.Remove(calendario);

            _context.LogAuditoria.Add(
            new LogAuditoria
            {
                EmailUsuario = User.Identity.Name,
                DetalhesAuditoria = string.Concat("Deletou o calendario:",
                   calendario.CalendarioId, "Data da deleção: ", DateTime.Now.ToLongDateString())
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Calendário de banca excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private bool CalendarioExists(int id)
        {
            return _context.Calendario.Any(e => e.CalendarioId == id);
        }
    }
}
