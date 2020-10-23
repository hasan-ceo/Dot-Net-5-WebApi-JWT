using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Tuteexy.Models;
using Tuteexy.Utility;

namespace Tuteexy.Service
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (_db.Roles.Any(r => r.Name == SD.Role_Admin)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@titan.com",
                Email = "admin@titan.com",
                EmailConfirmed = true,
                PhoneNumber = "+8801765263343",
                CreatedDate=DateTime.Now
            }, "Admin123!").GetAwaiter().GetResult();

            ApplicationUser user = _db.ApplicationUsers.Where(u => u.Email == "admin@titan.com").FirstOrDefault();

            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "azmainfiaz@gmail.com",
                Email = "azmainfiaz@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+8801765263343",
                CreatedDate = DateTime.Now
            }, "Admin123!").GetAwaiter().GetResult();

            user = _db.ApplicationUsers.Where(u => u.Email == "azmainfiaz@gmail.com").FirstOrDefault();

            _userManager.AddToRoleAsync(user, SD.Role_User).GetAwaiter().GetResult();

        }
    }
}
