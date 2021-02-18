using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set;}

        [DisplayName("Descrição do Status")]
        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "O campo {0} é obrigatorio.")]
        public string  DescStatus { get; set; }

        public ICollection<Tcc> Tccs { get; set; }
    }
}
