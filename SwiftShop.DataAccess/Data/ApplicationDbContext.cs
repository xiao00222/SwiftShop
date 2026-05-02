using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SwiftShop.Models;
namespace SwiftShop.DataAccess.Data
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
             
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    id = 1,
                    Name = "Action",
                    DisplayOrder = 1
                },
                new Category
                {
                    id = 2,
                    Name = "SciFi",
                    DisplayOrder = 2
                },
                new Category
                {
                    id = 3,
                    Name = "History",
                    DisplayOrder = 3
                }
                );
            modelBuilder.Entity<Product>().HasData(

                new Product
                {
                    Id = 12,
                    Title = "Dune",
                    Author = "Frank Herbert",
                    ISBN = "9780441013593",
                    ListPrice = 35,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryID = 2,
                    Description = "A desert planet. A chosen one. An empire at stake. Frank Herbert's Dune is the best-selling science fiction novel of all time.",
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780441013593-L.jpg"
                },
                new Product
                {
                    Id = 13,
                    Title = "Ender's Game",
                    Author = "Orson Scott Card",
                    ISBN = "9780812550702",
                    ListPrice = 30,
                    Price = 25,
                    Price50 = 22,
                    Price100 = 18,
                    CategoryID = 2,
                    Description = "A young genius is trained in a battle school in space to lead Earth's armies against an alien invasion.",
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780812550702-L.jpg"
                },

                new Product
                {
                    Id = 14,
                    Title = "The Martian",
                    Author = "Andy Weir",
                    ISBN = "9780553418026",
                    ListPrice = 28,
                    Price = 24,
                    Price50 = 20,
                    Price100 = 16,
                    CategoryID = 2,
                    Description = "An astronaut is stranded on Mars and must use science and ingenuity to survive until rescue.",
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780553418026-L.jpg"
                },

               new Product
               {
                   Id = 15,
                   Title = "Sapiens",
                   Author = "Yuval Noah Harari",
                   ISBN = "9780062316097",
                   ListPrice = 40,
                   Price = 35,
                   Price50 = 30,
                   Price100 = 25,
                   CategoryID = 3,
                   Description = "A brief history of humankind, from the Stone Age to the twenty-first century.",
                   ImageUrl = "https://covers.openlibrary.org/b/isbn/9780062316097-L.jpg"
               },

                new Product
                {
                    Id = 16,
                    Title = "The Hunger Games",
                    Author = "Suzanne Collins",
                    ISBN = "9780439023481",
                    ListPrice = 25,
                    Price = 20,
                    Price50 = 17,
                    Price100 = 14,
                    CategoryID = 1,
                    Description = "In a dystopian future, a girl volunteers to fight to the death in a televised competition to save her sister.",
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780439023481-L.jpg"
                },
                new Product
                {
                    Id = 17,
                    Title = "The Da Vinci Code",
                    Author = "Dan Brown",
                    ISBN = "9780307474278",
                    ListPrice = 30,
                    Price = 25,
                    Price50 = 22,
                    Price100 = 18,
                    CategoryID = 1,
                    Description = "A murder in the Louvre leads to a trail of clues hidden in the works of Leonardo da Vinci.",
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780307474278-L.jpg"
                });


        }
       
    }
}
