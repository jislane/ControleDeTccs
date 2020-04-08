using System;
using System.Collections.Generic;
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

        public DateTime  DataDeCadastro { get; set; }

        public Status Status { get; set; }
        public Banca Banca { get; set; }
        public Usuario Usuario { get; set; }
        public Calendario Calendario { get; set; }



    }
}
