using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class Banca
    {
        [Key]
        public int BancaId { get; set; }

        [DisplayName("Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public DateTime DataDeCadastro { get; set; }

        [Required]
        public Tcc Tcc { get; set; }

        [DisplayName("TCC")]
        public int TccId { get; set; }

        [Required]
        public Usuario Usuario { get; set; }

        [DisplayName("Usuário")]
        public string UsuarioId { get; set; }

        public TipoUsuario TipoUsuario { get; set; }

        [DisplayName("Tipo de Usuário")]
        public int TipoUsuarioId { get; set; }
        
        [DisplayName("Nota")]
        public double? Nota { get; set; }
    }
}
