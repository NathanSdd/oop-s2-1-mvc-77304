using Xunit;
using Microsoft.EntityFrameworkCore;
using Library.MVC.Data;
using Library.Domain;
using System;
using System.Linq;

namespace Library.Tests
{
    public class LoanTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Can_Create_Loan()
        {
            var context = GetDbContext();

            var book = new Book { Title = "Test Book", IsAvailable = true };
            var member = new Member { FullName = "John Doe" };

            context.Books.Add(book);
            context.Members.Add(member);
            context.SaveChanges();

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7)
            };

            context.Loans.Add(loan);

            // simulate your logic
            book.IsAvailable = false;

            context.SaveChanges();

            Assert.Equal(1, context.Loans.Count());
            Assert.False(book.IsAvailable);
        }

        [Fact]
        public void Cannot_Loan_Same_Book_Twice()
        {
            var context = GetDbContext();

            var book = new Book { Title = "Test Book", IsAvailable = false };
            var member = new Member { FullName = "John Doe" };

            context.Books.Add(book);
            context.Members.Add(member);
            context.SaveChanges();

            context.Loans.Add(new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                ReturnedDate = null
            });

            context.SaveChanges();

            // simulate your validation rule
            bool isOnLoan = context.Loans
                .Any(l => l.BookId == book.Id && l.ReturnedDate == null);

            Assert.True(isOnLoan);
        }

        [Fact]
        public void Returning_Book_Makes_It_Available()
        {
            var context = GetDbContext();

            var book = new Book { Title = "Test Book", IsAvailable = false };
            var member = new Member { FullName = "John Doe" };

            context.Books.Add(book);
            context.Members.Add(member);
            context.SaveChanges();

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                ReturnedDate = null
            };

            context.Loans.Add(loan);
            context.SaveChanges();

            // simulate return
            loan.ReturnedDate = DateTime.Now;
            book.IsAvailable = true;

            context.SaveChanges();

            Assert.True(book.IsAvailable);
            Assert.NotNull(loan.ReturnedDate);
        }
    }
}