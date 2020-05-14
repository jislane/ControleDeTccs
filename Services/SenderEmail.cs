using Microsoft.Extensions.Configuration;
using SistemaDeControleDeTCCs.Models;
using System;
using System.Net.Mail;

namespace SistemaDeControleDeTCCs.Services
{
    public class SenderEmail
    {
        private readonly SmtpClient _smtp;
        private readonly IConfiguration _configuration;

        public SenderEmail(SmtpClient smtp, IConfiguration configuration)
        {
            _smtp = smtp;
            _configuration = configuration;
        }

        public void EnviarSenhaParaUsuarioViaEmail(Usuario usuario, String senha)
        {
            string bodyEmail = string.Format("<h2>Conta Criada - Sistema de Controle de TCC</h2>" +
                "<p>Prezado(a), <b>{0}</b>! </p>" +
                "<p>Agora você possui uma conta no Sistema de Controle de TCC. Seguem os dados cadastrados. </p>" +
                "Tipo de Usuário: {1} <br/>" +
                "Nome: {2} <br/>" +
                "Matrícula: {3} <br/>" +
                "CPF: {4} <br/>" +
                "E-mail: {5} <br/>" +
                "Telefone: {6} <br/>" +
                "<p>Para acessar o sistema utilize a seguinte senha: <b>{7}</b></p>" +
                "<p><i>Não responda a esta mensagem. Este e-mail foi enviado por um sistema automático que não processa respostas.</i></p>",
                usuario.Nome,
                usuario.TipoUsuario.DescTipo,
                usuario.Nome,
                usuario.Matricula,
                usuario.cpf,
                usuario.email,
                usuario.telefone,
                senha);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            message.To.Add(usuario.email);
            message.Subject = "Dados de acesso - Sistema de Controle de TCC";
            message.Body = bodyEmail;
            message.IsBodyHtml = true;

            _smtp.Send(message);
        }
    }
}
