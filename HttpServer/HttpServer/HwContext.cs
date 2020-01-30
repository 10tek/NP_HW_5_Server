using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer
{
    public class HwContext : DbContext
    {
        public HwContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=A-205-07;Database=UserDb;Trusted_Connection=True;");
        }
    }
}
