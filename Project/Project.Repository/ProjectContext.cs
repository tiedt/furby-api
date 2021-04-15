using Microsoft.EntityFrameworkCore;
using Project.Domain;


namespace Project.Respository
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }

        public DbSet<Employee> Employees {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}