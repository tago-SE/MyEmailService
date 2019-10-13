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

        public async Task<Messege> OpenMessage(int? id, String username)
        {
            Messege message = await _context.Messeges.FirstOrDefaultAsync(m => m.MessageId == id);
            if (message.ToUser == username && message.MessegeState == MessegeState.Unread)
            {
                message.MessegeState = MessegeState.Read;
                _context.Update(message);
                await _context.SaveChangesAsync();
            }
            return message;
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

        public async Task<int> CountUserUnreadMessegesAsync(string username)
        {
            List<Messege> messeges = await _context.Messeges.Where(
                m => m.FromUser == username &&
                m.MessegeState == MessegeState.Unread).ToListAsync();
            return messeges.Count();
        }

        public Boolean MessageExists(int id)
        {
            return _context.Messeges.Any(e => e.MessageId == id);
        }

    }
}
