using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReadersConnect.Application.Helpers.Common;
using ReadersConnect.Domain.Models.Identity;
using ReadersConnect.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Infrastructure.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly CoreApplicationContext _coreApplicationContext;

        public DbInitializer(CoreApplicationContext coreApplicationContext)
        {
            _coreApplicationContext = coreApplicationContext;
        }
        public void InitializeDatabase()
        {
            // check for pending migrations to db and apply the migrations if they are not yet applied
            if (_coreApplicationContext.Database.GetPendingMigrations().Count() > 0)
            {
                _coreApplicationContext.Database.Migrate();
            }
        }
    }
}
