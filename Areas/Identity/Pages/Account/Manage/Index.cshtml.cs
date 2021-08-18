using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public IndexModel(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone(ErrorMessage = "Número inválido")]
            [Display(Name = "Telefone")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "O {0} é obrigatório", AllowEmptyStrings = false)]
            public string Nome { get; set; }

            [Required(ErrorMessage = "O {0} é obrigatório", AllowEmptyStrings = false)]
            public string Sobrenome { get; set; }

            [Display(Name = "E-mail")]
            [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
            [Required(ErrorMessage = "O {0} é obrigatório", AllowEmptyStrings = false)]
            public string Email { get; set; }

            [Display(Name = "Matrícula")]
            [Required(ErrorMessage = "A {0} é obrigatória", AllowEmptyStrings = false)]
            public string Matricula { get; set; }
        }

        private async Task LoadAsync(Usuario user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Nome = user.Nome,
                Sobrenome = user.Sobrenome,
                Email = user.Email,
                Matricula = user.Matricula
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            /*if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }*/

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var nome = (await _userManager.FindByNameAsync(user.UserName)).Nome;
            if (Input.Nome != nome)
            {
                user.Nome = Input.Nome;
                var result = await _userManager.UpdateAsync(user);
            }

            var sobrenome = (await _userManager.FindByNameAsync(user.UserName)).Sobrenome;
            if (Input.Sobrenome != sobrenome)
            {
                user.Sobrenome = Input.Sobrenome;
                var result = await _userManager.UpdateAsync(user);
            }

            var email = (await _userManager.FindByNameAsync(user.UserName)).Email;
            if (Input.Email != email)
            {
                user.Email = Input.Email;
                user.UserName = Input.Email;
                var result = await _userManager.UpdateAsync(user);
            }

            var matricula = (await _userManager.FindByNameAsync(user.UserName)).Matricula;
            if (Input.Matricula != matricula)
            {
                user.Matricula = Input.Matricula;
                var result = await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Seu perfil foi atualizado";
            return RedirectToPage();
        }
    }
}
