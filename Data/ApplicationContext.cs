using Microsoft.EntityFrameworkCore;
using VisiTrack.Models;

namespace VisiTrack.Data
{
    public class ApplicationContext:DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<VisiTrack.Models.Host> Hosts { get; set; }
        public DbSet<Visit> Visits { get; set; }
    }

}

