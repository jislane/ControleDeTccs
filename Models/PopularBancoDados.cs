using System;
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

            var statusPendente = new Status { DescStatus = "Pendente" };
            var statusHomologadoTCC = new Status { DescStatus = "Homologado TCC" };
            var statusHomologadoBanca = new Status { DescStatus = "Homologado Banca" };
            var statusCancelado = new Status { DescStatus = "Cancelado" };
            var statusAprovado = new Status { DescStatus = "Aprovado" };
            var statusReprovado = new Status { DescStatus = "Reprovado" };
            _context.Status.AddRange(statusAprovado, statusCancelado, statusHomologadoBanca, statusHomologadoTCC, statusPendente, statusReprovado);

            var calendario20191 = new Calendario { Ano = 2019, Semestre = 1, DataInicio = new DateTime(2019, 06, 03), DataFim = new DateTime(2019, 06, 14), Ativo = false };
            var calendario20201 = new Calendario { Ano = 2020, Semestre = 1, DataInicio = new DateTime(2020, 06, 01), DataFim = new DateTime(2020, 06, 12), Ativo = true };
            _context.Calendario.AddRange(calendario20191, calendario20201);

            var userAdm = new Usuario
            {
                email = "admin@ifs.edu.br",
                Nome = "Administrador do Sistema",
                Matricula = "0",
                TipoUsuario = tipoUsuarioAdm
            };
            var userCatuxe = new Usuario
            {
                email = "catuxe@ifs.edu.br",
                Nome = "Catuxe",
                Matricula = "143717",
                cpf = "48786338786",
                TipoUsuario = tipoUsuarioCoord,
                telefone = "99999999999"
            };
            var userJislane = new Usuario
            {
                email = "jislane@ifs.edu.br",
                Nome = "Jislane",
                Matricula = "413682",
                cpf = "61788703205",
                TipoUsuario = tipoUsuarioProf,
                telefone = "99999999999"
            };
            var userGilson = new Usuario
            {
                email = "gilson@ifs.edu.br",
                Nome = "Gilson",
                Matricula = "148915",
                cpf = "05578896130",
                TipoUsuario = tipoUsuarioProf,
                telefone = "99999999999"
            };
            var userGlauco = new Usuario
            {
                email = "glauco@ifs.edu.br",
                Nome = "Glauco",
                Matricula = "154790",
                cpf = "18780486144",
                TipoUsuario = tipoUsuarioProf,
                telefone = "99999999999"
            };
            var userJean = new Usuario
            {
                email = "jean@ifs.edu.br",
                Nome = "Jean",
                Matricula = "477854",
                cpf = "05875304515",
                TipoUsuario = tipoUsuarioProf,
                telefone = "99999999999"
            };
            var userWillian = new Usuario
            {
                email = "willian@ifs.edu.br",
                Nome = "Willian",
                Matricula = "20142863156328",
                cpf = "57878135484",
                TipoUsuario = tipoUsuarioAluno,
                telefone = "99999999999"
            };
            var userAlex = new Usuario
            {
                email = "alex@ifs.edu.br",
                Nome = "Alex",
                Matricula = "20151863147125",
                cpf = "05447800241",
                TipoUsuario = tipoUsuarioAluno,
                telefone = "99999999999"
            };
            var userAna = new Usuario
            {
                email = "ana@ifs.edu.br",
                Nome = "Ana Carla",
                Matricula = "20151863147165",
                cpf = "15787601551",
                TipoUsuario = tipoUsuarioAluno,
                telefone = "99999999999"
            };
            var userMateus = new Usuario
            {
                email = "mateus@ifs.edu.br",
                Nome = "Mateus",
                Matricula = "20131863058127",
                cpf = "04263417322",
                TipoUsuario = tipoUsuarioAluno,
                telefone = "99999999999"
            };
            var userHelena = new Usuario
            {
                email = "helena@ifs.edu.br",
                Nome = "Helena",
                Matricula = "20122863098617",
                cpf = "12364289838",
                TipoUsuario = tipoUsuarioAluno,
                telefone = "99999999999"
            };
            _context.Usuario.AddRange(userAdm, userWillian, userAlex, userAna, userCatuxe, userJislane, userGilson, userGlauco, userJean, userMateus, userHelena);

            var tccWillian = new Tcc {
                Tema = "Sistema X",
                Usuario = userWillian,
                DataDeCadastro = new DateTime(2020, 02, 13, 19, 35, 04),
                Status = statusPendente
            };
            var tccAlex = new Tcc {
                Tema = "Sistema Y",
                Usuario = userAlex,
                DataDeCadastro = new DateTime(2020, 03, 02, 21, 43, 12),
                Status = statusHomologadoTCC
            };
            var tccMateus = new Tcc
            {
                Tema = "Sistema M",
                Usuario = userMateus,
                DataDeCadastro = new DateTime(2017, 07, 07, 14, 27, 15),
                Status = statusHomologadoBanca,
                Resumo = "Resumo resumo resumo",
                LocalApresentacao = "Lab. 01, COINF",
                DataApresentacao = new DateTime(2018, 06, 19, 19, 30, 00)
            };
            var tccHelena = new Tcc
            {
                Tema = "Sistema H",
                Usuario = userHelena,
                DataDeCadastro = new DateTime(2016, 08, 11, 17, 04, 27),
                Status = statusCancelado,
                Resumo = "Resumo resumo resumo"
            };
            var tccAna = new Tcc
            {
                Tema = "Sistema Z",
                Usuario = userAna,
                DataDeCadastro = new DateTime(2019, 03, 11, 20, 08, 31),
                Status = statusAprovado,
                Resumo = "Resumo resumo resumo",
                LocalApresentacao = "Laboratório 06, COINF, IFS-Lagarto",
                DataApresentacao = new DateTime(2019, 06, 04, 20, 30, 52),
                DataFinalizacao = new DateTime(2019, 06, 14, 19, 34, 08),
                Nota = 8.5
            };
            _context.Tccs.AddRange(tccWillian, tccAlex, tccAna, tccMateus, tccHelena);

            var orientadorWillian = new Banca { Tcc = tccWillian, Usuario = userCatuxe, TipoUsuario = tipoUsuarioOrientador };
            var coorientadorWillian = new Banca { Tcc = tccWillian, Usuario = userGilson, TipoUsuario = tipoUsuarioCoorientador };
            var orientadorAlex = new Banca { Tcc = tccAlex, Usuario = userGilson, TipoUsuario = tipoUsuarioOrientador };
            var coorientadorAlex = new Banca { Tcc = tccAlex, Usuario = userJislane, TipoUsuario = tipoUsuarioCoorientador };
            var orientadorMateus = new Banca { Tcc = tccMateus, Usuario = userGlauco, TipoUsuario = tipoUsuarioOrientador };
            var coorientadorMateus = new Banca { Tcc = tccMateus, Usuario = userJean, TipoUsuario = tipoUsuarioCoorientador };
            var membroBancaMateus = new Banca { Tcc = tccMateus, Usuario = userGilson, TipoUsuario = tipoUsuarioMembroBanca };
            var orientadorAna = new Banca { Tcc = tccAna, Usuario = userJislane, TipoUsuario = tipoUsuarioOrientador, Nota = 9.0 };
            var coorientadorAna = new Banca { Tcc = tccAna, Usuario = userGilson, TipoUsuario = tipoUsuarioCoorientador, Nota = 8.5 };
            var membroBancaAna = new Banca { Tcc = tccAna, Usuario = userJean, TipoUsuario = tipoUsuarioMembroBanca, Nota = 8.0 };
            _context.Banca.AddRange(orientadorWillian, coorientadorWillian, orientadorAlex, coorientadorAlex, orientadorAna, coorientadorAna, membroBancaAna, orientadorMateus, coorientadorMateus, membroBancaMateus);

            _context.SaveChanges();
        }
    }
}
