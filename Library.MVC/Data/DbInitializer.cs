using Bogus;
using Library.Domain;

namespace Library.MVC.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Books.Any()) return; // already seeded

            // Books
            var bookFaker = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Isbn, f => f.Random.Replace("###-##########"))
                .RuleFor(b => b.Category, f => f.PickRandom("Fiction", "Science", "History"))
                .RuleFor(b => b.IsAvailable, true);

            var books = bookFaker.Generate(20);
            context.Books.AddRange(books);

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

            // pick random subset of books (no duplicates)
            var loanBooks = books.OrderBy(x => random.Next()).Take(10).ToList();

            var loans = new List<Loan>();

            foreach (var book in loanBooks)
            {
                var member = members[random.Next(members.Count)];

                var loan = new Loan
                {
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

            foreach (var loan in loans)
            {
                loan.ReturnedDate = null;

                var book = context.Books.Find(loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = false;
                }
            }

            context.Loans.AddRange(loans);
            context.SaveChanges();
        }
    }
}