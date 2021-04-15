using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeControleDeTCCs.Models
{
    [Table("LogAuditoria")]
    public class LogAuditoria
    {
        [Column("Id")]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Column("DetalhesAuditoria")]
        [Display(Name = "Detalhe Auditoria")]
        public string DetalhesAuditoria { get; set; }

        [Column("IdIten")]
        [Display(Name = "Id do item")]
        public string IdItem { get; set; }

        [Column("EmailUsuario")]
        [Display(Name = "Email Usuário")]
        public string EmailUsuario { get; set; }
        [Column("DataUpdate")]
        [Display(Name = "Data de auterações")]
        public string Date { get; set; }
        [Column("IpUsuario")]
        [Display(Name = "Ip do Usuário")]
        public string Ip { get; set; }

    }
}
