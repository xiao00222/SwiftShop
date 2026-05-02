using Microsoft.EntityFrameworkCore;
using SwiftShop.Models;

namespace SwiftShop.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    id = 1,
                    Name = "Test",
                    DisplayOrder = 1
                },
                new Category
                {
                    id = 2,
                    Name = "Test2",
                    DisplayOrder = 2
                },
                new Category
                {
                    id = 3,
                    Name = "Test3",
                    DisplayOrder = 3
                }
                );
        }
    }
}
