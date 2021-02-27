using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class Usuario : IdentityUser
    {
        [DisplayName("Nome")]
        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string Nome { get; set; }

        [DisplayName("Sobrenome")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        [MinLength(6, ErrorMessage = "{0} inválido!")]
        public string Sobrenome { get; set; }

        [DisplayName("Matrícula")]
        [Column(TypeName = "nvarchar(25)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string Matricula { get; set; }

        [DisplayName("CPF")]
        [Column(TypeName = "nvarchar(11)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string Cpf { get; set; }

        public TipoUsuario TipoUsuario { get; set; }

        [ForeignKey("TipoUsuario")]
        [DisplayName("Tipo de Usuário")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public int TipoUsuarioId { get; set; }

        public Curso Curso { get; set; }

        [DisplayName("Curso")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public int IdCurso { get; set; }
    }
}
