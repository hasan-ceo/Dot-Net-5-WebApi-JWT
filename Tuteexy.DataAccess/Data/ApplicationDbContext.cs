using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tuteexy.Models;

namespace Tuteexy.Service
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //Iron Man
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }


    }
}


