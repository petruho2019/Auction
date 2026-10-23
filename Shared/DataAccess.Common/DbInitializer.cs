
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Common;


public class DBInitializer
{
    public static async Task InitializeDatabase(DbContext context)
         => context.Database.EnsureCreated();
}