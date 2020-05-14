using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public Usuario Usuario { get; set; }
        public ICollection<TipoUsuario> TiposUsuario { get; set; }
    }
}
