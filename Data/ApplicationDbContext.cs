using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyEmailService.Models;

namespace MyEmailService.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Message> Messeges { get; set; }

        public DbSet<Login> Logins { get; set; }

        public DbSet<UserDetail> UserDetails { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }

    /*
    public class AppUser : IdentityUser
    {
        public int ReadMessegesCount { get; set; }
    }
    */
}
