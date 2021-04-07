using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class Calendario
    {
        [Key]
        public int CalendarioId { get; set; }

        [Display(Name = "Ano")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public int Ano { get; set; }

        [Display(Name = "Semestre")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public int Semestre { get; set; }

        [Display(Name = "Data Início")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data Fim")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        public DateTime DataFim { get; set; }

        [Display(Name = "Situação")]
        public Boolean Ativo { get; set; }
    }
}
