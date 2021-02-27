using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class Curso
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Nome")]
        [Column(TypeName = "nvarchar(30)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string Nome { get; set; }

        [DisplayName("Sigla")]
        [Column(TypeName = "nvarchar(10)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string Sigla { get; set; }
        
        [DisplayName("Código do Curso no MEC")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public int CodEmec { get; set; }

        public Usuario Coordenador { get; set; }

        [ForeignKey("Usuario")]
        [DisplayName("Coordenador")]
        public string IdCoordenador { get; set; }

        public Campus Campus { get; set; }

        [DisplayName("Campus")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public int IdCampus { get; set; }

    }
}
