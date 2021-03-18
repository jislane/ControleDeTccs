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

        [DisplayName("Tema")]
        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string Tema { get; set; }

        [DisplayName("Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public DateTime DataDeCadastro { get; set; }

        public Status Status { get; set; }

        [DisplayName("Status")]
        public int StatusId { get; set; }

        public Usuario Usuario { get; set; }

        [ForeignKey("Usuario")]
        [DisplayName("Discente")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string UsuarioId { get; set; }

        [DisplayName("Resumo")]
        public string Resumo { get; set; }

        [Display(Name = "Data de Apresentação")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataApresentacao { get; set; }

        [Display(Name = "Data de Finalização")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFinalizacao { get; set; }
        
        [Display(Name = "Nota")]
        public double? Nota { get; set; }

        [Display(Name = "Local da Apresentação")]
        public string LocalApresentacao { get; set; }

        public Curso Curso { get; set; }

        [DisplayName("Curso")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public int IdCurso { get; set; }
    }
}
