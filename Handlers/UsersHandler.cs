using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyEmailService.Data;
using MyEmailService.Models;
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

        public async Task OnLogin(IdentityUser user)
        {
            Login login = new Login(user);
            _context.Add(login);
            await _context.SaveChangesAsync();
        }

        /**
         * Returns the second last login attempt if existing
         */
        public async Task<DateTime> GetPreviousLoginAttempt(IdentityUser user)
        {
            List<Login> userLogins = await _context.Logins
                .Where(login => login.UserId == user.Id).ToListAsync();
            int size = userLogins.Count();
            if (size > 1) {
                return userLogins[size - 2].Timestamp;
            }
            return new DateTime();
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Where(u => u.Email == email).SingleAsync();
        }

        public async Task<int> GetNumLoginsThisMonth(IdentityUser user)
        {
            int month = DateTime.Now.Month;
            List<Login> userLogins = await _context.Logins
                .Where(login => login.UserId == user.Id && login.Timestamp.Month == month)
                .ToListAsync();
            return userLogins.Count();
        }

        private async Task<UserDetail> GetUserDetail(IdentityUser user)
        {
            string userId = user.Id;
            UserDetail detail = await _context.UserDetails.Where(d => d.UserId == userId).SingleOrDefaultAsync();
            if (detail == null)
            {
                detail = new UserDetail(user);
                _context.Add(detail);
                await _context.SaveChangesAsync();
            }
            return detail;
        }

        public async Task IncreaseUserReadMessegeCount(IdentityUser user)
        {
            UserDetail detail = await GetUserDetail(user);
            detail.ReadMessegeCount++;
            _context.Update(detail);
            await _context.SaveChangesAsync();
        }

        public async Task IncreaseUserDeletedMessegeCount(IdentityUser user)
        {
            UserDetail detail = await GetUserDetail(user);
            detail.DeletedMessegeCount++;
            _context.Update(detail);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserReadMesseges(IdentityUser user)
        {
            UserDetail detail = await GetUserDetail(user);
            return detail.ReadMessegeCount;
        }

        public async Task<int> GetUserDeletedMesseges(IdentityUser user)
        {
            UserDetail detail = await GetUserDetail(user);
            return detail.DeletedMessegeCount;
        }

        public Task<List<string>> GetUserNames()
        {
            return _context.Users.Select(u => u.UserName).ToListAsync();
        }

    }
}
