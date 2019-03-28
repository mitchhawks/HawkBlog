using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HawkBlog.Data;
using HawkBlog.Models;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace HawkBlog.Controllers
{
    public class BlogController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context, IOptionsSnapshot<HawkBlogSettings> settingsOptions) : base(settingsOptions)
        {
            _context = context;
        }

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

            float totalPosts = applicationDbContext.ToList().Count();

            float maxPage = totalPosts / pageSize;

            ViewData["maxPages"] = Math.Ceiling(maxPage);

            return View(await applicationDbContext.ToListAsync());
        }

        [Route("Blog/{year}/{month}/{slug}")]
        public async Task<IActionResult> ViewPost(int year, int month, string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            ViewData["CatList"] = GetCatList();

            var post = await _context.Post
                .Where(p => p.PostDatePub.Year == year && p.PostDatePub.Month == month && p.PostSlug.ToUpper().Equals(slug.ToUpper()))
                .Include(p => p.PostCategory)
                .SingleOrDefaultAsync();
            if (post == null)
            {
                return NotFound();
            }

            if(!post.isPublished && !User.IsInRole("Admin"))
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

            var cat = _context.Category
                .Where(m => m.CatUrlSlug.ToUpper() == catSlug.ToUpper())
                .FirstOrDefaultAsync().Result;

            if (cat == null)
            {
                return NotFound();
            }

            var posts = _context.Post
                .Where(p => p.PostCategory.CatName == cat.CatName && p.isPublished)
                .OrderByDescending(p => p.PostDatePub)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.PostCategory);

            ViewData["CatList"] = GetCatList();

            ViewData["CurrentCat"] = cat;

            float totalPosts = posts.ToList().Count();

            float maxPage = totalPosts / pageSize;

            ViewData["maxPages"] = Math.Ceiling(maxPage);

            ViewData["postCount"] = _context.Post.Where(p => p.PostCategory.CatName == cat.CatName).Count();

            return View("Index", await posts.ToListAsync());

        }

        [Route("Blog/Search")]
        public async Task<IActionResult> SearchPosts(string searchTerm, int page)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction("Index");
            }

            if (page == 0)
            {
                page = 1;
            }
            var skip = page * 5 - 5;

            ViewData["currentPage"] = page;

            var results = new List<Result>();

            var list = new List<Post>();

            IEnumerable<Post> posts;

            posts = _context.Post
                .Where(p => p.isPublished)
                .Include(p => p.PostCategory);


            /*
             Search Posts
             Snippet from the Blogifier Engine (https://github.com/blogifierdotnet/Blogifier)
             */
            foreach (var item in posts)
            {
                var rank = 0;
                var hits = 0;

                searchTerm = searchTerm.ToLower();

                if (item.PostTitle.ToLower().Contains(searchTerm))
                {
                    hits = Regex.Matches(item.PostTitle.ToLower(), searchTerm).Count;

                    rank += hits * 10;
                }
                if (item.PostShortDesc.ToLower().Contains(searchTerm))
                {
                    hits = Regex.Matches(item.PostShortDesc.ToLower(), searchTerm).Count;

                    rank += hits * 3;
                }
                if (item.PostContent.ToLower().Contains(searchTerm))
                {
                    rank += Regex.Matches(item.PostContent.ToLower(), searchTerm).Count;
                }

                if (rank > 0)
                {
                    results.Add(new Result { Rank = rank, Item = item });
                }
            }

            results = results.OrderByDescending(r => r.Rank).ToList();

            float totalPosts = results.ToList().Count();

            float maxPage = totalPosts / 5;

            ViewData["CatList"] = GetCatList();

            ViewData["maxPages"] = Math.Ceiling(maxPage);

            for (int i = 0; i < results.Count; i++)
            {
                list.Add(results[i].Item);
            }

            ViewData["SearchQuery"] = searchTerm;

            return View("Index", await Task.Run(() => list.Skip(skip).Take(5).ToList()));
        }

        public IList<Category> GetCatList()
        {
            var categories = _context.Category;
            return categories.ToList();
        }

        public class Result
        {
            public int Rank { get; set; }

            public Post Item { get; set; }
        }
    }
}
