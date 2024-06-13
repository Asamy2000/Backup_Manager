using Db.Bak.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace Db.Bak
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
    }
}
