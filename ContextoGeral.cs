using Microsoft.EntityFrameworkCore;

namespace SistemaDeControleDeTCCs.Models
{
    public class ContextoGeral: DbContext
    {
        public ContextoGeral()
        {

        }
        public ContextoGeral(DbContextOptions<ContextoGeral> options)
            : base(options)
        {
        }

    }
}