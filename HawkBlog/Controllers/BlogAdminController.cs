using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HawkBlog.Data;
using HawkBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HawkBlog.Controllers
{
    public class BlogAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Post
                .Include(p => p.PostCategory);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Blog/Create
        public IActionResult Create()
        {
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatName");
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostID,PostTitle,PostShortDesc,PostSlug,PostContent,isPublished,CatID")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.PostDatePub = DateTime.Now;
                post.PostLastModified = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatID", post.CatID);
            return View(post);
        }

        // GET: Blog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(m => m.PostID == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatName", post.CatID);
            return View(post);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostID,PostTitle,PostShortDesc,PostSlug,PostContent,PostDatePub,isPublished,CatID")] Post post)
        {
            if (id != post.PostID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.PostLastModified = DateTime.Now;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostID))
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
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatID", post.CatID);
            return View(post);
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.PostCategory)
                .SingleOrDefaultAsync(m => m.PostID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.SingleOrDefaultAsync(m => m.PostID == id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IList<Category> GetCatList()
        {
            var categories = _context.Category;
            return categories.ToList();
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.PostID == id);
        }
    }
}