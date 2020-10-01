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
using SistemaDeControleDeTCCs.Utils;

namespace SistemaDeControleDeTCCs.Controllers
{
    [Area("Coordenador")]
    [Authorize(Roles = "Administrador, Coordenador")]
    public class UsuariosController : Controller
    {
        private readonly SistemaDeControleDeTCCsContext _context;
        private readonly SenderEmail _senderEmail;

        public UsuariosController(SistemaDeControleDeTCCsContext context, SenderEmail senderEmail)
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
        public IActionResult AddOrEdit(string id)
        {
            var tiposUsuarios = _context.TipoUsuario.OrderBy(x => x.DescTipo).Where(x => x.DescTipo.Contains("Aluno") || x.DescTipo.Contains("Professor")).ToList();
            var usuario = new Usuario();
            if (id != null)
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
                if (usuario.Id != null)
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    
                }
                /*
                else
                {
                    _context.Add(usuario);
                    var senha = KeyGenerator.GetUniqueKey(8);
                    await _context.SaveChangesAsync();
                    usuario.TipoUsuario = _context.TipoUsuario.Where(x => x.TipoUsuarioId == usuario.TipoUsuarioId).FirstOrDefault();
                    _senderEmail.EnviarSenhaParaUsuarioViaEmail(usuario, senha);
                }
                */
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            _context.Users.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }       
    }
}
