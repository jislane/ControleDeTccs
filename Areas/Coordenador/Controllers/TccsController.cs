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
        public async Task<IActionResult> Index()
        {
            return View(_context.Tccs.ToList());
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
            ViewBag.ProfessorList = new SelectList(_context.Usuario.Where(x => x.TipoUsuarioId.Equals(5)).OrderBy(x => x.Nome), "Id", "Nome");
            var viewModel = new TccViewModel { Usuarios = discentes, Tcc = tcc };
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
