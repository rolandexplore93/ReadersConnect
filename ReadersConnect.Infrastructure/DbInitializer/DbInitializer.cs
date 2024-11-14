using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReadersConnect.Domain.Models.Identity;
using ReadersConnect.Infrastructure.Persistence;

namespace ReadersConnect.Infrastructure.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly CoreApplicationContext _coreApplicationContext;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbInitializer> _logger;
        private readonly string _firstname;
        private readonly string _lastname;
        private readonly string _username;
        private readonly string _email;
        private readonly string _password;

        public DbInitializer(CoreApplicationContext coreApplicationContext, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<DbInitializer> logger)
        {
            _coreApplicationContext = coreApplicationContext;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _firstname = _configuration["SuperAdmin:FirstName"];
            _lastname = _configuration["SuperAdmin:LastName"];
            _username = _configuration["SuperAdmin:UserName"];
            _email = _configuration["SuperAdmin:Email"];
            _password = _configuration["SuperAdmin:Password"];

        }
        public void InitializeDatabase()
        {
            // check for pending migrations to db and apply the migrations if they are not yet applied
            if (_coreApplicationContext.Database.GetPendingMigrations().Count() > 0)
            {
                _coreApplicationContext.Database.Migrate();
            }
        }

        // Create SuperAdmin User with superior priority when the app is launched
        public async Task SeedSuperAdminUserAsync()
        {
            try
            {
                // Create SuperAdmin role if it does not exist
                bool isRoleExists = await _roleManager.RoleExistsAsync("SuperAdmin");
                if (!isRoleExists)
                {
                    _logger.LogInformation("Creating SuperAdmin role...");
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                    _logger.LogInformation("SuperAdmin role created.");
                }

                // Create SuperAdmin user if it does not exists
                ApplicationUser isSuperAdminExists = await _userManager.FindByEmailAsync(_email);
                if (isSuperAdminExists == null)
                {
                    ApplicationUser user = new ApplicationUser()
                    {
                        FirstName = _firstname,
                        LastName = _lastname,
                        UserName = _username,
                        Email = _email,
                        EmailConfirmed = true,
                    };

                    _logger.LogInformation("Creating SuperAdmin user...");
                    var result = await _userManager.CreateAsync(user, _password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("SuperAdmin role created.");

                        // Assign SuperAdmin role to SuperAdmin user
                        _logger.LogInformation("Assigning SuperAdmin role to SuperAdmin user...");
                        await _userManager.AddToRoleAsync(user, "SuperAdmin");
                        _logger.LogInformation("SuperAdmin role assign to SuperAdmin user.");
                    }
                    else
                    {
                        _logger.LogError($"Error occurred creating SuperAdmin user. Error: {result.Errors}");
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured creating SuperAdmin user: {ex.Message}");
            }
        }
    }
}