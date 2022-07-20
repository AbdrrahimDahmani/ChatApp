using ChatApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser>Users { get; set; }
    }
}
