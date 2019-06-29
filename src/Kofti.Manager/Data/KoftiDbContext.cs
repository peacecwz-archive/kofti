using Kofti.Manager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kofti.Manager.Data
{
    public class KoftiDbContext : DbContext
    {
        public KoftiDbContext(DbContextOptions<KoftiDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationEntity> Applications { get; set; }
        public DbSet<ConfigEnttiy> Configs { get; set; }
    }
}