using System;
using System.Collections.Generic;
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
        public string  DescTipo { get; set; }

    }
}
