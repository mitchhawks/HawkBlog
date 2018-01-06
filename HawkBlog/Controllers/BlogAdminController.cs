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

        [Route("BlogAdmin/Post/Index")]
        public async Task<IActionResult> PostIndex()
        {
            var applicationDbContext = _context.Post
                .OrderByDescending(p => p.PostDatePub)
                .Include(p => p.PostCategory);

            return View("Post/PostIndex", await applicationDbContext.ToListAsync());
        }

        // GET: Blog/Create
        [Route("BlogAdmin/Post/NewPost")]
        public IActionResult NewPost()
        {
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatName");
            return View("Post/NewPost");
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("BlogAdmin/Post/NewPost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("PostID,PostTitle,PostShortDesc,PostSlug,PostContent,isPublished,CatID")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.PostDatePub = DateTime.Now;
                post.PostLastModified = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(PostIndex));
            }
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatID", post.CatID);
            return View("Post/NewPost", post);
        }

        [Route("BlogAdmin/Post/EditPost/{id}")]
        // GET: Blog/Edit/5
        public async Task<IActionResult> EditPost(int? id)
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
            return View("Post/EditPost", post);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("BlogAdmin/Post/EditPost/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, [Bind("PostID,PostTitle,PostShortDesc,PostSlug,PostContent,PostDatePub,isPublished,CatID")] Post post)
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
                return RedirectToAction(nameof(PostIndex));
            }
            ViewData["CatID"] = new SelectList(_context.Set<Category>(), "CatID", "CatID", post.CatID);
            return View("Post/EditPost", post);
        }

        [Route("BlogAdmin/Post/DeletePost/{id}")]
        // GET: Blog/Delete/5
        public async Task<IActionResult> DeletePost(int? id)
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

            return View("Post/DeletePost", post);
        }

        // POST: Blog/Delete/5
        [Route("BlogAdmin/Post/DeletePost/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Post.SingleOrDefaultAsync(m => m.PostID == id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PostIndex));
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