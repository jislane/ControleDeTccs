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
using SistemaDeControleDeTCCs.Utils;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ContextoGeral _context;
        private readonly SenderEmail _senderEmail;

        public UsuariosController(ContextoGeral context, SenderEmail senderEmail)
        {
            _context = context;
            _senderEmail = senderEmail;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToListAsync());
        }

        // GET: Usuarios/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            var tiposUsuarios = _context.TipoUsuario.OrderBy(x => x.DescTipo).Where(x => x.DescTipo.Contains("Aluno") || x.DescTipo.Contains("Professor")).ToList();
            var usuario = new Usuario();
            if (id != 0)
            {
                usuario = _context.Usuario.Find(id);
            }
            var viewModel = new UsuarioViewModel { TiposUsuario = tiposUsuarios, Usuario = usuario };
            return View(viewModel);
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("UsuarioId,Nome,Matricula,cpf,telefone,email,TipoUsuarioId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.UsuarioId == 0)
                {
                    _context.Add(usuario);
                    var senha = KeyGenerator.GetUniqueKey(8);
                    await _context.SaveChangesAsync();
                    usuario.TipoUsuario = _context.TipoUsuario.Where(x => x.TipoUsuarioId == usuario.TipoUsuarioId).FirstOrDefault();
                    _senderEmail.EnviarSenhaParaUsuarioViaEmail(usuario, senha);
                }
                else
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }       
    }
}
