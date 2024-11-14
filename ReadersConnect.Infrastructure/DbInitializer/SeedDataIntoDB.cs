using Microsoft.AspNetCore.Identity;
using ReadersConnect.Domain.Models.Identity;
using ReadersConnect.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadersConnect.Infrastructure.Persistence;

namespace ReadersConnect.Infrastructure.DbInitializer
{
    public class SeedDataIntoDb
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly CoreApplicationContext _context;

        public SeedDataIntoDb(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, CoreApplicationContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedBooksAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[] { "Librarian", "LibrarianAssistant", "Member", "SuperAdmin", "BookCollector" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "adminUser", Email = "admin@example.com", FirstName = "Admin", LastName = "User", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "librarianUser", Email = "librarian@example.com", FirstName = "Librarian", LastName = "User", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "assistant1", Email = "assistant1@example.com", FirstName = "Assistant", LastName = "One", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "assistant2", Email = "assistant2@example.com", FirstName = "Assistant", LastName = "Two", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "assistant3", Email = "assistant3@example.com", FirstName = "Assistant", LastName = "Three", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "member1", Email = "member1@example.com", FirstName = "Member", LastName = "One", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "member2", Email = "member2@example.com", FirstName = "Member", LastName = "Two", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "member3", Email = "member3@example.com", FirstName = "Member", LastName = "Three", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "member4", Email = "member4@example.com", FirstName = "Member", LastName = "Four", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true },
                new ApplicationUser { UserName = "member5", Email = "member5@example.com", FirstName = "Member", LastName = "Five", IsActive = true, EmailConfirmed = true, PhoneNumberConfirmed = true }
            };

            foreach (var user in users)
            {
                if (await _userManager.FindByEmailAsync(user.Email) == null)
                {
                    await _userManager.CreateAsync(user, "Admin@123*");

                    if (user.UserName == "adminUser")
                        await _userManager.AddToRoleAsync(user, "SuperAdmin");
                    else if (user.UserName == "librarianUser")
                        await _userManager.AddToRoleAsync(user, "Librarian");
                    else if (user.UserName.StartsWith("assistant"))
                        await _userManager.AddToRoleAsync(user, "LibrarianAssistant");
                    else
                        await _userManager.AddToRoleAsync(user, "Member");
                }
            }
        }

        private async Task SeedBooksAsync()
        {
            var books = new List<Book>
            {
                new Book { Title = "Book 1", Author = "Author 1", ISBN = "ISBN001", Genre = new List<string> { "Fiction", "Mystery" }, PublishedDate = DateTime.UtcNow, Copies = 3, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book1.jpg" },
                new Book { Title = "Book 2", Author = "Author 2", ISBN = "ISBN002", Genre = new List<string> { "Horror", "Romance" }, PublishedDate = DateTime.UtcNow, Copies = 1, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book2.jpg" },
                new Book { Title = "Book 3", Author = "Author 3", ISBN = "ISBN003", Genre = new List<string> { "Romance", "Thriller" }, PublishedDate = DateTime.UtcNow, Copies = 2, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book2.jpg" },
                new Book { Title = "Book 4", Author = "Author 4", ISBN = "ISBN004", Genre = new List<string> { "Dramatic", "Romance" }, PublishedDate = DateTime.UtcNow, Copies = 5, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book2.jpg" },
                new Book { Title = "Book 5", Author = "Author 5", ISBN = "ISBN005", Genre = new List<string> { "Horror", "Horror" }, PublishedDate = DateTime.UtcNow, Copies = 3, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book2.jpg" },
                new Book { Title = "Book 6", Author = "Author 6", ISBN = "ISBN006", Genre = new List<string> { "Battle", "Thriller" }, PublishedDate = DateTime.UtcNow, Copies = 2, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book2.jpg" },
                new Book { Title = "Book 7", Author = "Author 7", ISBN = "ISBN007", Genre = new List<string> { "Battle", "Fiction" }, PublishedDate = DateTime.UtcNow, Copies = 3, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book2.jpg" },
                new Book { Title = "Book 8", Author = "Author 8", ISBN = "ISBN008", Genre = new List<string> { "Horror", "Dramatic" }, PublishedDate = DateTime.UtcNow, Copies = 4, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book2.jpg" },
                new Book { Title = "Book 9", Author = "Author 9", ISBN = "ISBN009", Genre = new List<string> { "Horror", "Thriller" }, PublishedDate = DateTime.UtcNow, Copies = 6, ImageUrl = "https://placeholder.com/100", ImageLocalPath = "/images/book2.jpg" },
                
            };

            foreach (var book in books)
            {
                if (!_context.Books.Any(b => b.Title == book.Title))
                {
                    _context.Books.Add(book);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
