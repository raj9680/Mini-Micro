using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data
{
    public class DBContext: DbContext
    {
        public DBContext(DbContextOptions<DBContext> options):base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasData(
                new Course { Id=1, CourseName="DotNet", CourseDescription="Sample Description", Price=2345 },
                new Course { Id=2, CourseName = "SpringBoot", CourseDescription = "Sample Description", Price = 1600 });
        }

        // Tables
        public DbSet<Course> Courses { get; set; }
    }
}
