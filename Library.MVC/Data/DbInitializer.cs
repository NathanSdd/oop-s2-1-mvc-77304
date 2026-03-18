using Bogus;
using Library.Domain;

namespace Library.MVC.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Books.Any() || context.Members.Any() || context.Loans.Any())
            {
                return; // DB already seeded
            }
            // Clear existing data if needed
            //context.Loans.RemoveRange(context.Loans);
            //context.Books.RemoveRange(context.Books);
            //context.Members.RemoveRange(context.Members);
            //context.SaveChanges();

            //  Books
            var bookFaker = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Commerce.ProductName())
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Isbn, f => f.Random.Replace("###-##########"))
                .RuleFor(b => b.Category, f => f.PickRandom("Fiction", "Science", "History"))
                .RuleFor(b => b.IsAvailable, true);

            var books = bookFaker.Generate(20);
            context.Books.AddRange(books);
            context.SaveChanges(); 

            // Members
            var memberFaker = new Faker<Member>()
                .RuleFor(m => m.FullName, f => f.Name.FullName())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber());

            var members = memberFaker.Generate(10);
            context.Members.AddRange(members);
            context.SaveChanges(); 

            // Loans (unique books only)
            var random = new Random();

            var loanBooks = books
                .OrderBy(x => random.Next())
                .Take(10)
                .ToList();

            var loans = new List<Loan>();

            foreach (var book in loanBooks)
            {
                var member = members[random.Next(members.Count)];

                var loan = new Loan
                {
                    // DO NOT SET Id
                    BookId = book.Id,
                    MemberId = member.Id,
                    LoanDate = DateTime.Now.AddDays(-random.Next(1, 10)),
                    DueDate = DateTime.Now.AddDays(random.Next(5, 15)),
                    ReturnedDate = null
                };

                book.IsAvailable = false;

                loans.Add(loan);
            }

            context.Loans.AddRange(loans);
            context.SaveChanges();
        }
    }
}