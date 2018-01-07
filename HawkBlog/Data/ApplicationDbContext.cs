using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HawkBlog.Models;

namespace HawkBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Post>()
            .HasOne(p => p.PostCategory)
            .WithMany(b => b.PostsInCat)
            .OnDelete(DeleteBehavior.ClientSetNull);

        }

        public DbSet<HawkBlog.Models.Post> Post { get; set; }

        public DbSet<HawkBlog.Models.Category> Category { get; set; }
    }
}
