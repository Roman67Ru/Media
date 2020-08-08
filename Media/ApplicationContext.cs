using System.Data.Entity;

namespace Media
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DBConnection")
        {
        }
        public DbSet<TVProgram> TVPrograms { get; set; }
        public DbSet<VideoCards> VideoCards { get; set; }
    }
}
