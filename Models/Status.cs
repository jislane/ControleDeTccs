using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeControleDeTCCs.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set;}

        [Column(TypeName = "nvarchar(250)")]
        public string  DescStatus { get; set; }

        public ICollection<Tcc> Tccs { get; set; }
    }
}
