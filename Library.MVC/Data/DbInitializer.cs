using Bogus;
using Library.Domain;
using Microsoft.AspNetCore.Identity;


namespace Library.MVC.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // CLEAR DATA
            context.Loans.RemoveRange(context.Loans);
            context.Books.RemoveRange(context.Books);
            context.Members.RemoveRange(context.Members);
            context.SaveChanges();

            // BOOKS (20)
            var books = new List<Book>();

            for (int i = 1; i <= 20; i++)
            {
                books.Add(new Book
                {
                    Title = $"Book {i}",
                    Author = $"Author {i}",
                    Category = "Fiction",
                    Isbn = $"ISBN-{i}",
                    IsAvailable = true
                });
            }

            context.Books.AddRange(books);

            // MEMBERS (10)
            var members = new List<Member>();

            for (int i = 1; i <= 10; i++)
            {
                members.Add(new Member
                {
                    FullName = $"Member {i}",
                    Email = $"member{i}@test.com",
                    Phone = $"12345678{i}"
                });
            }

            context.Members.AddRange(members);

            context.SaveChanges();

            // LOANS (15)
            var loans = new List<Loan>();

            for (int i = 0; i < 15; i++)
            {
                var book = books[i]; // unique books (NO duplicates)
                var member = members[i % members.Count];

                DateTime loanDate = DateTime.Now.AddDays(-i);

                DateTime? returnedDate = null;

                // CONTROL LOGIC
                if (i < 5)
                {
                    // returned loans
                    returnedDate = loanDate.AddDays(5);
                }
                else if (i < 10)
                {
                    // active loans (NOT returned)
                    returnedDate = null;
                }
                else
                {
                    // overdue loans
                    returnedDate = null;
                }

                var loan = new Loan
                {
                    BookId = book.Id,
                    MemberId = member.Id,
                    LoanDate = loanDate,
                    DueDate = loanDate.AddDays(7),
                    ReturnedDate = returnedDate
                };

                // update availability
                if (returnedDate == null)
                {
                    book.IsAvailable = false;
                }

                loans.Add(loan);
            }

            context.Loans.AddRange(loans);

            context.SaveChanges();
        }

        public static async Task SeedAdmin(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create Admin user
        var admin = await userManager.FindByEmailAsync("admin@test.com");

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com"
            };

            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
}