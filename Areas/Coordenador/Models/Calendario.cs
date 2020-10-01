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
        [Required(ErrorMessage = "O {0} é obrigatório")]
        public int Ano { get; set; }
        [Required(ErrorMessage = "O {0} é obrigatório")]
        public int Semestre { get; set; }
        [Display(Name = "Data Início")]
        [Required(ErrorMessage = "A {0} é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        public DateTime DataInicio { get; set; }
        [Display(Name = "Data Fim")]
        [Required(ErrorMessage = "A {0} é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        public DateTime DataFim { get; set; }
        public Boolean Ativo { get; set; }
    }
}
