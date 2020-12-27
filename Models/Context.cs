using Microsoft.EntityFrameworkCore;

namespace Login.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options){}
        public DbSet<Users> Users{get;set;}   
        public DbSet<Weddings> Weddings{get;set;}
        public DbSet<Guests> Guests{get;set;}
    }
}