using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SistemaDeControleDeTCCs.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Services
{
    public class SenderEmail : IEmailSender
    {
        private readonly SmtpClient _smtp;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string host;
        private int port;
        private bool enableSSL;
        private string userName;
        private string password;

        public SenderEmail(string host, int port, bool enableSSL, string userName, string password)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.userName = userName;
            this.password = password;
        }

        public SenderEmail(SmtpClient smtp, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _smtp = smtp;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            
        }

        public void EnviarSenhaParaUsuarioViaEmail(Usuario usuario, String senha)
        {
            string bodyEmail = string.Format("<h2>Conta Criada - Sistema de Controle de TCC</h2>" +
                "<p>Prezado(a), <b>{0}</b>! </p>" +
                "<p>Agora você possui uma conta no Sistema de Controle de TCC. Seguem os dados cadastrados. </p>" +
                "Tipo de Usuário: {1} <br/>" +
                "Nome: {0} {2} <br/>" +
                "Matrícula: {3} <br/>" +
                "CPF: {4} <br/>" +
                "E-mail: {5} <br/>" +
                "Telefone: {6} <br/>" +
                "<p>Para acessar o sistema utilize a seguinte senha: <b>{7}</b></p>" +
                "<p>Cordialmente,</p>" +
                "<p><b>SISTEMA DE CONTROLE DE TCC</b> <br/>" +
                "Coordenação do Bacharelado em Sistemas de Informação - CBSI <br/>" +
                "Instituto Federal de Sergipe - Campus Lagarto - IFS</p>" +
                "<p><i>Não responda a esta mensagem. Este e-mail foi enviado por um sistema automático que não processa respostas.</i></p>",
                usuario.Nome,
                usuario.TipoUsuario.DescTipo,
                usuario.Sobrenome,
                usuario.Matricula,
                usuario.Cpf,
                usuario.Email,
                usuario.PhoneNumber,
                senha);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            message.To.Add(usuario.Email);
            message.Subject = "Dados de acesso - Sistema de Controle de TCC";
            message.Body = bodyEmail;
            message.IsBodyHtml = true;

            _smtp.Send(message);
        }

        public void NotificarDiscenteCadastroTCCViaEmail(Usuario usuario, String titulo)
        {
            string bodyEmail = string.Format("<p>Prezado(a), <b>{0}</b>! </p>" +
                "<p>Informamos que o seu Trabalho de Conclusão de Curso (TCC) intitulado\"<b>{1}</b>\" foi cadastrado no Sistema de Controle de TCC. A partir de agora você poderá adicionar as informações complementares (como o resumo etc). </p>" +
                "<p>Cordialmente,</p>" +
                "<p><b>SISTEMA DE CONTROLE DE TCC</b> <br/>" +
                "Coordenação do Bacharelado em Sistemas de Informação - CBSI <br/>" +
                "Instituto Federal de Sergipe - Campus Lagarto - IFS</p>" +
                "<p><i>Não responda a esta mensagem. Este e-mail foi enviado por um sistema automático que não processa respostas.</i></p>",
                usuario.Nome,
                titulo);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            message.To.Add(usuario.Email);
            message.Subject = "Cadastro de TCC - Sistema de Controle de TCC";
            message.Body = bodyEmail;
            message.IsBodyHtml = true;

            _smtp.Send(message);
        }

        public void NotificarMembrosBancaViaEmail(Tcc tcc, Usuario usuario, String fileID)
        {
            //Exemplo Download 
            ///localhost:51077/FileTCCs/Download/605a9a65-10ae-45de-a8b8-e9c2ab0e690c
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;
            string url = host + "/FileTCCs/Download/" + fileID;
            string bodyEmail = string.Format("<p>Prezado(a), <b>{0}</b>! </p>" +
                "<p>Seguem abaixo os dados de defesa do Trabalho de Conclusão de Curso (TCC) intitulado \"<b>{1}</b>\", de autoria do discente <b>{2}</b>.</p>" +
                "<p>Data: {3}</p>" +
                "<p>Local: {4}</p>" +
                "<p>Link Download TCC: <a href=http://" + url + ">{5}</a></p>" +
                "<p>Cordialmente,</p>" +
                "<p><b>SISTEMA DE CONTROLE DE TCC</b> <br/>" +
                "Coordenação do Bacharelado em Sistemas de Informação - CBSI <br/>" +
                "Instituto Federal de Sergipe - Campus Lagarto - IFS</p>" +
                "<p><i>Não responda a esta mensagem. Este e-mail foi enviado por um sistema automático que não processa respostas.</i></p>",
                usuario.Nome,
                tcc.Tema,
                tcc.Usuario.Nome + " " + tcc.Usuario.Sobrenome,
                tcc.DataApresentacao,
                tcc.LocalApresentacao,
                tcc.Tema);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            message.To.Add(usuario.Email);
            message.Subject = "Dados de Defesa do TCC";
            message.Body = bodyEmail;
            message.IsBodyHtml = true;

            _smtp.Send(message);
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = enableSSL
            };
            return client.SendMailAsync(
                new MailMessage(userName, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
}
