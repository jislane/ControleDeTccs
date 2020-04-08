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

        public ICollection<Tcc> Tccs { get; set; }

    }
}
