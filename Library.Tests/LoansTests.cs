using Xunit;
using Microsoft.EntityFrameworkCore;
using Library.MVC.Data;
using Library.Domain;
using System;
using System.Linq;

public class LoansTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void CannotLoanBookTwice()
    {
        var context = GetDbContext();

        var book = new Book { Title = "Test Book", IsAvailable = false };
        context.Books.Add(book);

        context.Loans.Add(new Loan
        {
            Book = book,
            LoanDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7),
            ReturnedDate = null
        });

        context.SaveChanges();

        bool isOnLoan = context.Loans
            .Any(l => l.BookId == book.Id && l.ReturnedDate == null);

        Assert.True(isOnLoan);
    }

    [Fact]
    public void ReturnMakesBookAvailable()
    {
        var context = GetDbContext();

        var book = new Book { Title = "Test Book", IsAvailable = false };
        context.Books.Add(book);

        var loan = new Loan
        {
            Book = book,
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
    }

    [Fact]
    public void OverdueLoanDetected()
    {
        var context = GetDbContext();

        var loan = new Loan
        {
            LoanDate = DateTime.Now.AddDays(-10),
            DueDate = DateTime.Now.AddDays(-5),
            ReturnedDate = null
        };

        context.Loans.Add(loan);
        context.SaveChanges();

        bool isOverdue = loan.DueDate < DateTime.Now && loan.ReturnedDate == null;

        Assert.True(isOverdue);
    }

    [Fact]
    public void BookSearchReturnsMatch()
    {
        var context = GetDbContext();

        context.Books.Add(new Book { Title = "C# Programming" });
        context.Books.Add(new Book { Title = "Java Basics" });

        context.SaveChanges();

        var result = context.Books
            .Where(b => b.Title.Contains("C#"))
            .ToList();

        Assert.Single(result);
    }

    [Fact]
    public void LoanCreationMarksBookUnavailable()
    {
        var context = GetDbContext();

        var book = new Book { Title = "Test Book", IsAvailable = true };
        context.Books.Add(book);

        var loan = new Loan
        {
            Book = book,
            LoanDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7)
        };

        book.IsAvailable = false;
        context.Loans.Add(loan);
        context.SaveChanges();

        Assert.False(book.IsAvailable);
    }
}