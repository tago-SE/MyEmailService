using Microsoft.EntityFrameworkCore;
using MyEmailService.Data;
using MyEmailService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.Handlers
{
    public class MessegesHandler
    {
        private readonly ApplicationDbContext _context;

        public MessegesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Messege> SendMessageAsync(String fromUser, String toUser, String title, String content)
        {
            Messege message = new Messege
            {
                TimeSent = DateTime.Now,
                Title = title,
                Content = content,
                FromUser = fromUser,
                ToUser = toUser
            };
            _context.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Messege> OpenMessageAsync(int? id, string username)
        {
            Messege messege = await _context.Messeges.FirstOrDefaultAsync(m => m.MessageId == id);
            if (messege.ToUser == username && messege.MessegeState == MessegeState.Unread)
            {
                messege.MessegeState = MessegeState.Read;
                _context.Update(messege);
                await _context.SaveChangesAsync();
                messege.WasRecentlyRead = true;
            }
            return messege;
        }

        public async Task<Messege> GetMessege(int id)
        {
            return await _context.Messeges
                .FirstOrDefaultAsync(m => m.MessageId == id);
        }

        public async Task DeleteMessegeAsync(int id)
        {
            var message = await _context.Messeges.FindAsync(id);
            _context.Messeges.Remove(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Messege>> GetAllMessagesAsync()
        {
            return await _context.Messeges.ToListAsync();
        }

        public async Task<List<Messege>> GetReceivedMessagesAsync(string username)
        {
            return await _context.Messeges.Where(m => m.ToUser == username).ToListAsync();
        }
        public async Task<List<Messege>> GetDeliveredMessagesAsync(string username)
        {
            return await _context.Messeges.Where(m => m.FromUser == username).ToListAsync();
        }

        public async Task<List<Messege>> GetInboxMessegesFromUser(string inboxUser, string fromUser)
        {
            return await _context.Messeges.Where(m => m.FromUser == fromUser && m.ToUser == inboxUser).ToListAsync();
        }

        public List<string> GetSenderNamesFromInbox(string inboxUser)
        {
            return _context.Messeges.Select(m => m.FromUser).ToList().Distinct().ToList();
        }

        public async Task<int> CountUserUnreadMessegesAsync(string username)
        {
            List<Messege> messeges = await _context.Messeges.Where(
                m => m.ToUser == username &&
                m.MessegeState == MessegeState.Unread).ToListAsync();
            return messeges.Count();
        }

        public async Task<int> CountReceivedUserMessegesAsync(string username)
        {
            return (await _context.Messeges.Where(m => m.ToUser == username).ToListAsync()).Count();
        }

        public bool MessageExists(int id)
        {
            return _context.Messeges.Any(e => e.MessageId == id);
        }

    }
}
