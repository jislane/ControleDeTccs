using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class Tcc
    {
        [Key]
        public int TccId { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "Este campo e obrigatório!")]
        public string Tema { get; set; }
        public DateTime DataDeCadastro { get; set; }
        public Status Status { get; set; }
        public int StatusId { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("Usuario")]
        [DisplayName("Discente")]
        [Required(ErrorMessage = "O {0} é obrigatório!")]
        public string UsuarioId { get; set; }
        public string Resumo { get; set; }
        public DateTime? DataApresentacao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public double? Nota { get; set; }
        public string? LocalApresentacao { get; set; }
    }
}
