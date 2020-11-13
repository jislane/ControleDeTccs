using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models.ViewModels
{
    public class TccViewModel
    {
        public Tcc Tcc { get; set; }
        public ICollection<Tcc> Tccs { get; set; }
        public ICollection<Usuario> Usuarios { get; set; }
        public ICollection<Banca> Banca { get; set; }
    }
}
