using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Infrastructure.DbInitializer
{
    public interface IDbInitializer
    {
        void InitializeDatabase();
    }
}
