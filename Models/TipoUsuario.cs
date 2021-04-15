using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class TipoUsuario
    {
        [Key]
        public int TipoUsuarioId { get; set; }

        [DisplayName("Descrição do Tipo de Usuário")]
        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string  DescTipo { get; set; }

    }
}
