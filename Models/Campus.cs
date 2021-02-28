using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class Campus
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Nome")]
        [Column(TypeName = "nvarchar(40)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string Nome { get; set; }

        [DisplayName("Endereço")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string Endereco { get; set; }
    }
}
