using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HawkBlog.Data;
using HawkBlog.Models;

namespace HawkBlog.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Blog
        public async Task<IActionResult> Index(int page)
        {
            if (page == 0)
            {
                page = 1;
            }
            int pageSize = 5;

            ViewData["currentPage"] = page;

            ViewData["CatList"] = GetCatList();

            var applicationDbContext = _context.Post
                .Where(p => p.isPublished)
                .OrderByDescending(p => p.PostDatePub)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.PostCategory);

            float totalPosts = applicationDbContext.ToList().Count() + 1;

            float maxPage = totalPosts / pageSize;

            ViewData["maxPages"] = Math.Ceiling(maxPage);

            return View(await applicationDbContext.ToListAsync());
        }

        [Route("Blog/Post/{year}/{month}/{slug}")]
        public async Task<IActionResult> ViewPost(int year, int month, string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Post
                .Where(p => p.PostDatePub.Year == year && p.PostDatePub.Month == month && p.PostSlug.Equals(slug))
                .Include(p => p.PostCategory)
                .SingleOrDefaultAsync();
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Route("Blog/Catagory/{catSlug}")]
        public async Task<IActionResult> GetPostsForCat(string catSlug, int page)
        {
            if (page == 0)
            {
                page = 1;
            }
            int pageSize = 5;
            ViewData["currentPage"] = page;

            if (string.IsNullOrEmpty(catSlug))
            {
                return NotFound();
            }

            string cat = _context.Category
                .Where(m => m.CatUrlSlug == catSlug)
                .FirstOrDefaultAsync().Result.CatName;

            if (cat == null)
            {
                return NotFound();
            }

            var posts = _context.Post
                .Where(p => p.PostCategory.CatName == cat)
                .OrderByDescending(p => p.PostDatePub)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.PostCategory);

            ViewData["CatList"] = GetCatList();

            float totalPosts = posts.ToList().Count() + 1;

            float maxPage = totalPosts / pageSize;

            ViewData["maxPages"] = Math.Ceiling(maxPage);

            ViewData["postCount"] = _context.Post.Where(p => p.PostCategory.CatName == cat).Count();

            return View("Index", await posts.ToListAsync());

        }

        // GET: Blog/Create
        public IActionResult Create()
        {
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatID");
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostID,PostTitle,PostShortDesc,PostSlug,PostContent,PostDatePub,PostLastModified,isPublished,CatID")] Post post)
        {
            if (ModelState.IsValid)
            {
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
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatID", post.CatID);
            return View(post);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostID,PostTitle,PostShortDesc,PostSlug,PostContent,PostDatePub,PostLastModified,isPublished,CatID")] Post post)
        {
            if (id != post.PostID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
