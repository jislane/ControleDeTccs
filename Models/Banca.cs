using System;
using System.Collections.Generic;
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
        public DateTime DataDeCadastro { get; set; }
        public Tcc Tcc { get; set; }
        public Usuario Usuario { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public double Nota { get; set; }

    }
}
