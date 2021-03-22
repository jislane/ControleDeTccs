using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models.ViewModels
{
    public class BancaViewModel
    {
        public int BancaId { get; set; }
        public List<Banca> Membros { get; set; }

    }
}
