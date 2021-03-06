using Microsoft.AspNetCore.Identity;
using SistemaDeControleDeTCCs.Data;
using System;
using System.Linq;

namespace SistemaDeControleDeTCCs.Models
{
    public class PopularBancoDados
    {
        private readonly SistemaDeControleDeTCCsContext _context;

        public PopularBancoDados(SistemaDeControleDeTCCsContext context)
        {
            _context = context;
        }

        public void PopularProducao()
        {
            if (_context.TipoUsuario.Any())
            {
                return;
            }

            var roleAdm = new IdentityRole { Name = "Administrador", NormalizedName = "ADMINISTRADOR" };
            var roleAluno = new IdentityRole { Name = "Aluno", NormalizedName = "ALUNO" };
            var roleProf = new IdentityRole { Name = "Professor", NormalizedName = "PROFESSOR" };
            var roleCoord = new IdentityRole { Name = "Coordenador", NormalizedName = "COORDENADOR" };

            _context.Roles.AddRange(roleAdm, roleProf, roleCoord, roleAluno);

            var tipoUsuarioAdm = new TipoUsuario { DescTipo = "Administrador" };
            var tipoUsuarioAluno = new TipoUsuario { DescTipo = "Aluno" };
            var tipoUsuarioProf = new TipoUsuario { DescTipo = "Professor" };
            var tipoUsuarioCoord = new TipoUsuario { DescTipo = "Coordenador" };
            var tipoUsuarioOrientador = new TipoUsuario { DescTipo = "Orientador" };
            var tipoUsuarioCoorientador = new TipoUsuario { DescTipo = "Coorientador" };
            var tipoUsuarioMembroBanca = new TipoUsuario { DescTipo = "Membro da Banca" };
            _context.TipoUsuario.AddRange(tipoUsuarioAdm, tipoUsuarioAluno, tipoUsuarioCoord, tipoUsuarioCoorientador, tipoUsuarioMembroBanca, tipoUsuarioOrientador, tipoUsuarioProf);

            var CampusLag = new Campus { Nome = "Campus Lagarto", Endereco = "Lagarto" };

            var CursoBsi = new Curso { Nome = "Bacharelado", Sigla = "BSI", CodEmec = 00001, Campus = CampusLag };
            _context.Cursos.AddRange(CursoBsi);

            var userAdm = new Usuario
            {
                UserName = "admin@ifs.edu.br",
                NormalizedUserName = "admin@ifs.edu.br",
                Email = "admin@ifs.edu.br",
                NormalizedEmail = "admin@ifs.edu.br",
                Nome = "Administrador",
                Sobrenome = "do Sistema",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "0",
                TipoUsuario = tipoUsuarioAdm,
                Cpf = "99999999999",
                PhoneNumber = "99999999999",
                Curso = CursoBsi

            };

            _context.Usuario.AddRange(userAdm);

            var rowAdm = new IdentityUserRole<string>();
            rowAdm.UserId = userAdm.Id;
            rowAdm.RoleId = roleAdm.Id;
 
            _context.UserRoles.AddRange(rowAdm);

            _context.SaveChanges();
        }


            public void Popular()
        {
            if (_context.TipoUsuario.Any())
            {
                return;
            }

            var roleAdm = new IdentityRole { Name = "Administrador", NormalizedName = "ADMINISTRADOR" };
            var roleAluno = new IdentityRole { Name = "Aluno", NormalizedName = "ALUNO" };
            var roleProf = new IdentityRole { Name = "Professor", NormalizedName = "PROFESSOR" };
            var roleCoord = new IdentityRole { Name = "Coordenador", NormalizedName = "COORDENADOR" };

            _context.Roles.AddRange(roleAdm, roleProf, roleCoord, roleAluno);

            var tipoUsuarioAdm = new TipoUsuario { DescTipo = "Administrador" };
            var tipoUsuarioAluno = new TipoUsuario { DescTipo = "Aluno" };
            var tipoUsuarioProf = new TipoUsuario { DescTipo = "Professor" };
            var tipoUsuarioCoord = new TipoUsuario { DescTipo = "Coordenador" };
            var tipoUsuarioOrientador = new TipoUsuario { DescTipo = "Orientador" };
            var tipoUsuarioCoorientador = new TipoUsuario { DescTipo = "Coorientador" };
            var tipoUsuarioMembroBanca = new TipoUsuario { DescTipo = "Membro da Banca" };
            _context.TipoUsuario.AddRange(tipoUsuarioAdm, tipoUsuarioAluno, tipoUsuarioCoord, tipoUsuarioCoorientador, tipoUsuarioMembroBanca, tipoUsuarioOrientador, tipoUsuarioProf);

            var CampusLag = new Campus { Nome = "Campus Lagarto", Endereco = "Lagarto" };
            var CampusIta = new Campus { Nome = "Campus Itabaiana", Endereco = "Itabaiana" };
            var CampusEst = new Campus { Nome = "Campus Estância", Endereco = "Estância" };
            var CampusAju = new Campus { Nome = "Campus Aracaju", Endereco = "Aracaju" };
            var CampusGlo = new Campus { Nome = "Campus Glória", Endereco = "Glória" };
            _context.Campus.AddRange(CampusLag, CampusIta, CampusEst, CampusAju, CampusGlo);

            var CursoBsi = new Curso { Nome = "Bacharelado", Sigla = "BSI", CodEmec = 00001, Campus = CampusLag };
            _context.Cursos.AddRange(CursoBsi);

            var statusPendente = new Status { DescStatus = "Cadastrado" };
            var statusHomologadoTCC = new Status { DescStatus = "Pré-Homologado" };
            var statusHomologadoBanca = new Status { DescStatus = "Homologado Banca" };
            var statusCancelado = new Status { DescStatus = "Cancelado" };
            var statusAprovado = new Status { DescStatus = "Aprovado" };
            var statusReprovado = new Status { DescStatus = "Reprovado" };
            _context.Status.AddRange(statusAprovado, statusCancelado, statusHomologadoBanca, statusHomologadoTCC, statusPendente, statusReprovado);

            var calendario20191 = new Calendario { Ano = 2019, Semestre = 1, DataInicio = new DateTime(2019, 06, 03), DataFim = new DateTime(2019, 06, 14), Ativo = false };
            var calendario20201 = new Calendario { Ano = 2020, Semestre = 1, DataInicio = new DateTime(2020, 06, 01), DataFim = new DateTime(2020, 06, 12), Ativo = true };
            _context.Calendario.AddRange(calendario20191, calendario20201);

            // A senha de todos os usuários abaixo é: P@$$w0rd

            var userAdm = new Usuario
            {
                UserName = "admin@ifs.edu.br",
                NormalizedUserName = "admin@ifs.edu.br",
                Email = "admin@ifs.edu.br",
                NormalizedEmail = "admin@ifs.edu.br",
                Nome = "Administrador",
                Sobrenome = "do Sistema",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "0",
                TipoUsuario = tipoUsuarioAdm,
                Cpf = "99999999999",
                PhoneNumber = "99999999999",
                Curso = CursoBsi

            };
            var userCatuxe = new Usuario
            {
                UserName = "catuxe@ifs.edu.br",
                NormalizedUserName = "catuxe@ifs.edu.br",
                Email = "catuxe@ifs.edu.br",
                NormalizedEmail = "catuxe@ifs.edu.br",
                Nome = "Catuxe",
                Sobrenome = "Varjão",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "143717",
                Cpf = "48786338786",
                TipoUsuario = tipoUsuarioCoord,
                PhoneNumber = "99999999999",
                Curso = CursoBsi

            };
            var userJislane = new Usuario
            {
                UserName = "jislane@ifs.edu.br",
                NormalizedUserName = "jislane@ifs.edu.br",
                Email = "jislane@ifs.edu.br",
                NormalizedEmail = "jislane@ifs.edu.br",
                Nome = "Jislane",
                Sobrenome = "Silva",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "413682",
                Cpf = "61788703205",
                TipoUsuario = tipoUsuarioProf,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            var userGilson = new Usuario
            {
                UserName = "gilson@ifs.edu.br",
                NormalizedUserName = "gilson@ifs.edu.br",
                Email = "gilson@ifs.edu.br",
                NormalizedEmail = "gilson@ifs.edu.br",
                Nome = "Gilson",
                Sobrenome = "Santos",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "148915",
                Cpf = "05578896130",
                TipoUsuario = tipoUsuarioProf,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            var userGlauco = new Usuario
            {
                UserName = "glauco@ifs.edu.br",
                NormalizedUserName = "glauco@ifs.edu.br",
                Email = "glauco@ifs.edu.br",
                NormalizedEmail = "glauco@ifs.edu.br",
                Nome = "Glauco",
                Sobrenome = "Carvalho",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "154790",
                Cpf = "18780486144",
                TipoUsuario = tipoUsuarioProf,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            var userJean = new Usuario
            {
                UserName = "jean@ifs.edu.br",
                NormalizedUserName = "jean@ifs.edu.br",
                Email = "jean@ifs.edu.br",
                NormalizedEmail = "jean@ifs.edu.br",
                Nome = "Jean",
                Sobrenome = "Louis",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "477854",
                Cpf = "05875304515",
                TipoUsuario = tipoUsuarioProf,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            var userWillian = new Usuario
            {
                UserName = "willian@ifs.edu.br",
                NormalizedUserName = "willian@ifs.edu.br",
                Email = "willian@ifs.edu.br",
                NormalizedEmail = "willian@ifs.edu.br",
                Nome = "Willian",
                Sobrenome = "Farias",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "20142863156328",
                Cpf = "57878135484",
                TipoUsuario = tipoUsuarioAluno,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            var userAlex = new Usuario
            {
                UserName = "alex@ifs.edu.br",
                NormalizedUserName = "alex@ifs.edu.br",
                Email = "alex@ifs.edu.br",
                NormalizedEmail = "alex@ifs.edu.br",
                Nome = "Alex",
                Sobrenome = "Oliveira",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "20151863147125",
                Cpf = "05447800241",
                TipoUsuario = tipoUsuarioAluno,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            var userAna = new Usuario
            {
                UserName = "ana@ifs.edu.br",
                NormalizedUserName = "ana@ifs.edu.br",
                Email = "ana@ifs.edu.br",
                NormalizedEmail = "ana@ifs.edu.br",
                Nome = "Ana Carla",
                Sobrenome = "Nascimento",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "20151863147165",
                Cpf = "15787601551",
                TipoUsuario = tipoUsuarioAluno,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            var userMateus = new Usuario
            {
                UserName = "mateus@ifs.edu.br",
                NormalizedUserName = "mateus@ifs.edu.br",
                Email = "mateus@ifs.edu.br",
                NormalizedEmail = "mateus@ifs.edu.br",
                Nome = "Mateus",
                Sobrenome = "Lima",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "20131863058127",
                Cpf = "04263417322",
                TipoUsuario = tipoUsuarioAluno,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            var userHelena = new Usuario
            {
                UserName = "helena@ifs.edu.br",
                NormalizedUserName = "helena@ifs.edu.br",
                Email = "helena@ifs.edu.br",
                NormalizedEmail = "helena@ifs.edu.br",
                Nome = "Helena",
                Sobrenome = "Mendonça",
                PasswordHash = "AQAAAAEAACcQAAAAEHWjbOIHqtSUeaxyCj/PR6rYwJ/xgW8jl8TYjeC4+iDj4EsSkQogl1TYPTRDpUA/5g==",
                SecurityStamp = "WCSIGCB6MEQY4LMWJ7VFWYC43BJ6GNBD",
                ConcurrencyStamp = "4f1ea59f-a987-45cf-abe6-d9e456b43fae",
                LockoutEnabled = true,
                AccessFailedCount = 1,
                Matricula = "20122863098617",
                Cpf = "12364289838",
                TipoUsuario = tipoUsuarioAluno,
                PhoneNumber = "99999999999",
                Curso = CursoBsi
            };
            _context.Usuario.AddRange(userAdm, userCatuxe, userWillian, userAlex, userAna, userJislane, userGilson, userGlauco, userJean, userMateus, userHelena);

           
            var rowAdm = new IdentityUserRole<string>();
            rowAdm.UserId = userAdm.Id;
            rowAdm.RoleId = roleAdm.Id;
            var rowCoord = new IdentityUserRole<string>();
            rowCoord.UserId = userCatuxe.Id;
            rowCoord.RoleId = roleCoord.Id;
            var rowCoordProf = new IdentityUserRole<string>();
            rowCoordProf.UserId = userCatuxe.Id;
            rowCoordProf.RoleId = roleProf.Id;
            var rowJislane = new IdentityUserRole<string>();
            rowJislane.UserId = userJislane.Id;
            rowJislane.RoleId = roleProf.Id;
            var rowGilson = new IdentityUserRole<string>();
            rowGilson.UserId = userGilson.Id;
            rowGilson.RoleId = roleProf.Id;
            var rowGlauco = new IdentityUserRole<string>();
            rowGlauco.UserId = userGlauco.Id;
            rowGlauco.RoleId = roleProf.Id;
            var rowJean = new IdentityUserRole<string>();
            rowJean.UserId = userJean.Id;
            rowJean.RoleId = roleProf.Id;
            var rowWillian = new IdentityUserRole<string>();
            rowWillian.UserId = userWillian.Id;
            rowWillian.RoleId = roleAluno.Id;
            var rowAlex = new IdentityUserRole<string>();
            rowAlex.UserId = userAlex.Id;
            rowAlex.RoleId = roleAluno.Id;
            var rowAna = new IdentityUserRole<string>();
            rowAna.UserId = userAna.Id;
            rowAna.RoleId = roleAluno.Id;
            var rowMateus = new IdentityUserRole<string>();
            rowMateus.UserId = userMateus.Id;
            rowMateus.RoleId = roleAluno.Id;
            var rowHelena = new IdentityUserRole<string>();
            rowHelena.UserId = userHelena.Id;
            rowHelena.RoleId = roleAluno.Id;
            _context.UserRoles.AddRange(rowAdm, rowCoord, rowCoordProf, rowJislane, rowGilson, rowGlauco, rowJean, rowWillian, rowAlex, rowAna, rowMateus, rowHelena);
            
            _context.SaveChanges();
            
        }
    }
}
