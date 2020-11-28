using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<Usuario> _userManager;

        public UsuariosController(SistemaDeControleDeTCCsContext context, SenderEmail senderEmail, UserManager<Usuario> userManager)
        {
            _context = context;
            _senderEmail = senderEmail;
            _userManager = userManager;
        }

        // GET: Usuarios
        public IActionResult Index(string filterNome, string filterMatriculaCPF, int filterTipoUsuario)
        {
            List<Usuario> usuarios = _context.Usuario.ToList();
            // filtros
            if (!string.IsNullOrEmpty(filterNome))
            {
                usuarios = usuarios.Where(x => x.Nome.ToUpper().Contains(filterNome.ToUpper())).ToList();
                ViewData["filterNome"] = filterNome;
            }
            if (!string.IsNullOrEmpty(filterMatriculaCPF))
            {
                usuarios = usuarios.Where(x => x.Cpf.Contains(filterMatriculaCPF) || x.Matricula.Contains(filterMatriculaCPF)).ToList();
                ViewData["filterMatriculaCPF"] = filterMatriculaCPF;
            }
            if (filterTipoUsuario > 0)
            {
                usuarios = usuarios.Where(x => x.TipoUsuarioId == filterTipoUsuario).ToList();
                ViewBag.TipoUsuario = new SelectList(_context.TipoUsuario.Where(x => x.TipoUsuarioId == 1 || x.TipoUsuarioId == 4 || x.TipoUsuarioId == 5 || x.TipoUsuarioId == 6).OrderBy(x => x.DescTipo).ToList(), "TipoUsuarioId", "DescTipo", filterTipoUsuario);
            }
            else
            {
                ViewBag.TipoUsuario = new SelectList(_context.TipoUsuario.Where(x => x.TipoUsuarioId == 1 || x.TipoUsuarioId == 4 || x.TipoUsuarioId == 5 || x.TipoUsuarioId == 6).OrderBy(x => x.DescTipo).ToList(), "TipoUsuarioId", "DescTipo");
            }
            return View(usuarios.OrderBy(x => x.Nome));
        }

        // GET: Usuarios/Create
        public IActionResult AddOrEdit(string id)
        {
            var tiposUsuarios = _context.TipoUsuario.OrderBy(x => x.DescTipo).Where(x => x.DescTipo.Contains("Aluno") || x.DescTipo.Contains("Professor") || x.DescTipo.Contains("Coordenador")).ToList();
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
        public async Task<IActionResult> AddOrEdit([Bind("Id,Nome,Sobrenome,Matricula,Cpf,PhoneNumber,Email,TipoUsuarioId")] Usuario usuario)        
        {
                      
            if (ModelState.IsValid)                
            {
                if (usuario.Id != null)
                {
                    // _context.Update(usuario);
                    // await _context.SaveChangesAsync();
                    
                    var userTemp = _userManager.FindByIdAsync(usuario.Id).Result;
                    userTemp.Nome = usuario.Nome;
                    userTemp.Sobrenome = usuario.Sobrenome;
                    userTemp.Matricula = usuario.Matricula;
                    userTemp.Cpf = usuario.Cpf;
                    userTemp.PhoneNumber = usuario.PhoneNumber;
                    userTemp.Email = usuario.Email;
                    userTemp.TipoUsuarioId = usuario.TipoUsuarioId;
                    var user = await _userManager.UpdateAsync(userTemp);

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
            var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToArray();
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
