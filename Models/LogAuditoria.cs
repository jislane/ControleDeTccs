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

        [Column("EmailUsuario")]
        [Display(Name = "Email Usuário")]
        public string EmailUsuario { get; set; }
    }
}
