using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class FileTCC
    {
        [Key]
        public System.Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Extension { get; set; }
        
        [Required]
        public System.DateTime DataCadastro { get; set; }
        
        [Required]
        public long Length { get; set; }
        
        [Required]
        public byte[] FileStream { get; set; }
        
        [Required] 
        public Tcc Tcc { get; set; }

        public int TccId { get; set; }
    }
}
