using Microsoft.AspNetCore.Identity;
using MyEmailService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.Handlers
{

    public class UsersHandler
    {
        private readonly ApplicationDbContext _context;

        public UsersHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public IdentityUser FindSingleByUserName(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Single();
        }

        public List<string> GetUserNames()
        {
            return _context.Users.Select(u => u.UserName).ToList();
        }
    }
}
