using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReadersConnect.Domain.Models;
using ReadersConnect.Domain.Models.Identity;

namespace ReadersConnect.Infrastructure.Persistence
{
    public class CoreApplicationContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Used to save and QUery Instances of Ur entities using (UnitOfWOrk + Repository Pattern)
        /// </summary>
        /// <param name="options"></param>
        public CoreApplicationContext(DbContextOptions<CoreApplicationContext> options) : base(options)
        {
                
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Permission> Permissions {  get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
