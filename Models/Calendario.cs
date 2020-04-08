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

        public DateTime  DataApresentacao { get; set; }

        public ICollection<Tcc> Tccs { get; set; }
    }
}
