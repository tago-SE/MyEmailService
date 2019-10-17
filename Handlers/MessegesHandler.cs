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
    public class MessegesHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly UsersHandler _usersHandler;

        public MessegesHandler(ApplicationDbContext context)
        {
            _context = context;
            _usersHandler = new UsersHandler(context);
        }

        public async Task<Message> SendMessageAsync(string fromUser, string toUser, String title, String content)
        {
            IdentityUser sender = await _usersHandler.GetUserByEmailAsync(fromUser);
            IdentityUser receiver = await _usersHandler.GetUserByEmailAsync(toUser);
            if (sender != null && receiver != null)
            {
                Message message = new Message
                {
                    TimeSent = DateTime.Now,
                    Title = title,
                    Content = content,
                    FromUser = fromUser,
                    Sender = sender,
                    SenderId = sender.Id,
                    ToUser = toUser,
                    Receiver = receiver, 
                    ReceiverId = receiver.Id,
                };
                _context.Add(message);
                await _context.SaveChangesAsync();
                return message;
            }
            return null;
        }

        public async Task<Message> OpenMessageAsync(int? id, string username)
        {
            Message messege = await _context.Messeges.FirstOrDefaultAsync(m => m.MessageId == id);
            if (messege.ToUser == username && messege.MessageState == MessageState.Unread)
            {
                messege.MessageState = MessageState.Read;
                _context.Update(messege);
                await _context.SaveChangesAsync();
                messege.WasRecentlyRead = true;
            }
            return messege;
        }

        public async Task<Message> GetMessege(int id)
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

        public async Task<List<Message>> GetAllMessagesAsync()
        {
            return await _context.Messeges.ToListAsync();
        }

        public async Task<List<Message>> GetReceivedMessagesAsync(string username)
        {
            return await _context.Messeges.Where(m => m.ToUser == username).ToListAsync();
        }
        public async Task<List<Message>> GetDeliveredMessagesAsync(string username)
        {
            return await _context.Messeges.Where(m => m.FromUser == username).ToListAsync();
        }

        public async Task<List<Message>> GetInboxMessegesFromUser(string inboxUser, string fromUser)
        {
            return await _context.Messeges.Where(m => m.FromUser == fromUser && m.ToUser == inboxUser).ToListAsync();
        }

        public async Task<List<Message>> GetInboxMessegesFromUser(string inboxUser, List<string> senders)
        {
            return await _context.Messeges
               .Where(m => m.ToUser == inboxUser && senders.Contains(m.FromUser))
               .ToListAsync();
        }

        public async Task<List<string>> GetSenderNamesFromInbox(string username)
        {
            return await _context.Messeges.Where(m => m.ToUser == username).Select(m => m.FromUser).Distinct().ToListAsync();
        }

        public async Task<int> CountUserUnreadMessegesAsync(string username)
        {
            List<Message> messeges = await _context.Messeges.Where(
                m => m.ToUser == username &&
                m.MessageState == MessageState.Unread).ToListAsync();
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
