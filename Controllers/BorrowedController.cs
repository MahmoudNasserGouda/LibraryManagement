using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;

namespace Library.Controllers
{
    public class BorrowedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Borrowed
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Borrowed.Include(b => b.BorrowedBook).Include(b => b.BorrowingMember);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Borrowed/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Book, "BookId", "Title");
            ViewData["MemberId"] = new SelectList(_context.Member, "Id", "UserName");
            return View();
        }

        // POST: Borrowed/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,BookId")] Borrowed borrowed)
        {
            if (ModelState.IsValid)
            {
                var book = await _context.Book.FirstOrDefaultAsync(m => m.BookId == borrowed.BookId);
                var member = await _context.Member.FirstOrDefaultAsync(m => m.Id == borrowed.MemberId);

                borrowed.BorrowingMember = member;
                borrowed.BorrowedBook = book;
                if (book != null)
                {
                    if (book.AvailableCopies > 0)
                    {
                        book.AvailableCopies -= 1;
                        _context.Update(book);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("BookId", $"Sorry, There is no more copies of this book at the moment");
                    }
                }

                borrowed.BorrowedAt = DateTime.Now;
                _context.Add(borrowed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Book, "BookId", "Title", borrowed.BookId);
            ViewData["MemberId"] = new SelectList(_context.Member, "Id", "UserName", borrowed.MemberId);
            return View(borrowed);
        }

        // GET: Borrowed/Edit/5
        public async Task<IActionResult> Edit(int? BookId, int? MemberId)
        {
            if (BookId == null || MemberId == null || _context.Borrowed == null)
            {
                return NotFound();
            }

            var borrowed = await _context.Borrowed.FindAsync(MemberId, BookId);
            if (borrowed == null)
            {
                return NotFound();
            }
            var book = await _context.Book.FirstOrDefaultAsync(m => m.BookId == borrowed.BookId);

            try
            {
                if (book != null)
                {
                    borrowed.IsReturned = true;
                    book.AvailableCopies += 1;
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                _context.Remove(borrowed);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowedExists(borrowed.MemberId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        private bool BorrowedExists(int id)
        {
            return _context.Borrowed.Any(e => e.MemberId == id);
        }
    }
}
