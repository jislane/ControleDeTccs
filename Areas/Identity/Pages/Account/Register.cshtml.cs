using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;
using SistemaDeControleDeTCCs.Services;
using SistemaDeControleDeTCCs.Utils;

namespace SistemaDeControleDeTCCs.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Administrador, Coordenador")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;
        private RoleManager<IdentityRole> _roleManager;
        private readonly SistemaDeControleDeTCCsContext _context;
        private readonly SenderEmail _senderEmail;

        public RegisterModel(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            SistemaDeControleDeTCCsContext context,
            SenderEmail senderEmail
            //IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _senderEmail = senderEmail;
            //_emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O {0} é obrigatório", AllowEmptyStrings = false)]
            public string Nome { get; set; }

            [Required(ErrorMessage = "O {0} é obrigatório", AllowEmptyStrings = true)]
            public string Sobrenome { get; set; }

            [Required(ErrorMessage = "A {0} é obrigatória", AllowEmptyStrings = false)]
            [Display(Name = "Matrícula")]
            public string Matricula { get; set; }

            [Display(Name = "CPF")]
            [Required(ErrorMessage = "A {0} é obrigatória", AllowEmptyStrings = false)]
            [ValidacaoPersonalizadaCPF(ErrorMessage = "CPF inválido")]
            public string Cpf { get; set; }

            [Display(Name = "Tipo de Usuário")]
            [Required(ErrorMessage = "O {0} é obrigatório", AllowEmptyStrings = false)]
            public int TipoUsuarioId { get; set; }

            [Display(Name = "Curso")]
            [Required(ErrorMessage = "O {0} é obrigatório", AllowEmptyStrings = false)]
            public int IdCurso { get; set; }

            [Display(Name = "Telefone")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "O {0} é obrigatório", AllowEmptyStrings = false)]
            [EmailAddress(ErrorMessage = "Informe um e-mail válido")]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirme a Senha")]
            [Compare("Senha", ErrorMessage = "A senha e a senha de confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            // pass the TipoUsuario List using ViewData
            ViewData["tiposUsuarios"] = _context.TipoUsuario.OrderBy(x => x.DescTipo).Where(x => x.DescTipo.Contains("Aluno") || x.DescTipo.Contains("Professor")).ToList();
            ViewData["cursos"] = _context.Cursos.OrderBy(x => x.Nome).ToList();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");


            // search role
            var nameTipoUsuario = _context.TipoUsuario.Find(Input.TipoUsuarioId).DescTipo;
            //var nameTipoUsuario = "Aluno";
            var role = _roleManager.FindByNameAsync(nameTipoUsuario).Result;

            ViewData["tiposUsuarios"] = _context.TipoUsuario.OrderBy(x => x.DescTipo).Where(x => x.DescTipo.Contains("Aluno") || x.DescTipo.Contains("Professor") || x.DescTipo.Contains("Coordenador")).ToList();
            ViewData["cursos"] = _context.Cursos.OrderBy(x => x.Nome).ToList();

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var u = await _userManager.FindByNameAsync(Input.Email);
            if (u != null)
            {
                ModelState.AddModelError(string.Empty, "O e-mail já encontra-se cadastrado no sistema");
            }
            // Existe dois erro no model que é o da senha que não está sendo informada
            // Existe um bug em ValidacaoPersonalizadaCPF no campo do cpf, talvez seja js faltando
            // o  bug consiste que a requisição é enviada mesmo com cpf inválido
            // por isso essa condição
            if (ModelState.ErrorCount < 3)
            {
                
                var user = new Usuario
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Nome = Input.Nome,
                    Sobrenome = Input.Sobrenome,
                    Matricula = Input.Matricula,
                    Cpf = ValidateCpf.RemoveNaoNumericos(Input.Cpf),
                    PhoneNumber = Input.PhoneNumber,
                    TipoUsuarioId = Input.TipoUsuarioId,
                    IdCurso = Input.IdCurso
                };

                var senha = KeyGenerator.GetUniqueKey(8);

                var result = await _userManager.CreateAsync(user, senha);
                if (result.Succeeded)
                {
                    //_logger.LogInformation("User created a new account with password.");

                    // code for adding user to role
                    await _userManager.AddToRoleAsync(user, role.Name);

                    /*
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    */

                    StatusMessage = "Conta criada e enviado e-mail com a senha de acesso.";
                    user.TipoUsuario = _context.TipoUsuario.Where(x => x.TipoUsuarioId == user.TipoUsuarioId).FirstOrDefault();
                    user.Curso = _context.Cursos.Where(x => x.Id == user.IdCurso).FirstOrDefault();
                    _senderEmail.EnviarSenhaParaUsuarioViaEmail(user, senha);
                    //_logger.LogInformation("User created a new account with password ("+ senha +").");
                    return RedirectToPage();
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
