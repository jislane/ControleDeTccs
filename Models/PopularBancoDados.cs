using System.Linq;

namespace SistemaDeControleDeTCCs.Models
{
    public class PopularBancoDados
    {
        private readonly ContextoGeral _context;

        public PopularBancoDados(ContextoGeral context)
        {
            _context = context;
        }

        public void Popular()
        {
            if (_context.TipoUsuario.Any())
            {
                return;
            }

            var tipoUsuarioAdm = new TipoUsuario { DescTipo = "Administrador" };
            var tipoUsuarioAluno = new TipoUsuario { DescTipo = "Aluno" };
            var tipoUsuarioProf = new TipoUsuario { DescTipo = "Professor" };
            var tipoUsuarioCoord = new TipoUsuario { DescTipo = "Coordenador" };
            var tipoUsuarioOrientador = new TipoUsuario { DescTipo = "Orientador" };
            var tipoUsuarioCoorientador = new TipoUsuario { DescTipo = "Coorientador" };
            var tipoUsuarioMembroBanca = new TipoUsuario { DescTipo = "Membro da Banca" };
            _context.TipoUsuario.AddRange(tipoUsuarioAdm, tipoUsuarioAluno, tipoUsuarioCoord, tipoUsuarioCoorientador, tipoUsuarioMembroBanca, tipoUsuarioOrientador, tipoUsuarioProf);

            _context.SaveChanges();
        }
    }
}
