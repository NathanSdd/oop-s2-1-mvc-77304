using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;

namespace Library.MVC.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var loans = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member);

            return View(await loans.ToListAsync());
        }

        public IActionResult Create()
        {
            // Only available books
            var availableBooks = _context.Books.Where(b => b.IsAvailable);

            ViewData["BookId"] = new SelectList(availableBooks, "Id", "Title");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,MemberId")] Loan loan)
        {
            // Prevent double loan
            bool isOnLoan = _context.Loans
                .Any(l => l.BookId == loan.BookId && l.ReturnedDate == null);

            if (isOnLoan)
            {
                ModelState.AddModelError("", "This book is already on loan.");
            }

            if (ModelState.IsValid)
            {
                loan.LoanDate = DateTime.Now;
                loan.DueDate = DateTime.Now.AddDays(14);

                // Mark book unavailable
                var book = await _context.Books.FindAsync(loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = false;
                }

                _context.Add(loan);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", loan.MemberId);

            return View(loan);
        }

        // RETURN BOOK
        public async Task<IActionResult> Return(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan != null)
            {
                loan.ReturnedDate = DateTime.Now;

                // Make book available again
                var book = await _context.Books.FindAsync(loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = true;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (loan == null) return NotFound();

            return View(loan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan != null)
            {
                // restore availability
                var book = await _context.Books.FindAsync(loan.BookId);
                if (book != null && loan.ReturnedDate == null)
                {
                    book.IsAvailable = true;
                }

                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}