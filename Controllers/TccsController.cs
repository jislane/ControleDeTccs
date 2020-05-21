using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Models;
using SistemaDeControleDeTCCs.Models.ViewModels;
using SistemaDeControleDeTCCs.Services;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class TccsController : Controller
    {
        private readonly ContextoGeral _context;
        private readonly SenderEmail _senderEmail;

        public TccsController(ContextoGeral context, SenderEmail senderEmail)
        {
            _context = context;
            _senderEmail = senderEmail;
        }

        // GET: Tccs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tccs.ToListAsync());
        }

        // GET: Tccs/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            var discentes = _context.Usuario.Where(x => x.TipoUsuario.DescTipo.Contains("Aluno")).ToList();
            var tcc = new Tcc();
            if (id != 0)
            {
                tcc = _context.Tccs.Find(id);
            }
            var viewModel = new TccViewModel { Usuarios = discentes, Tcc = tcc };
            return View(viewModel);
        }

        // POST: Tccs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TccId,Tema,UsuarioId,DataDeCadastro")] Tcc tcc)
        {
            if (ModelState.IsValid)
            {
                if (tcc.TccId == 0)
                {
                    tcc.DataDeCadastro = DateTime.Now;
                    tcc.Status = _context.Status.Where(x => x.DescStatus.Contains("Pendente")).FirstOrDefault();
                    _context.Add(tcc);
                    await _context.SaveChangesAsync();
                    var discente = _context.Usuario.Where(x => x.UsuarioId == tcc.UsuarioId).FirstOrDefault();
                    _senderEmail.NotificarDiscenteCadastroTCCViaEmail(discente, tcc.Tema);
                }
                else
                {
                    _context.Update(tcc);
                    await _context.SaveChangesAsync();
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
