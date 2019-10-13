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

        public async Task SendMessageAsync(String fromUser, String toUser, String title, String content)
        {
            Message message = new Message
            {
                TimeSent = DateTime.Now,
                Title = title,
                Content = content,
                FromUser = fromUser,
                ToUser = toUser
            };
            _context.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<Message> OpenMessage(int? id, String username)
        {
            Message message = await _context.Messages.FirstOrDefaultAsync(m => m.MessageId == id);
            if (message.ToUser == username && message.MessageState == MessageState.Unread)
            {
                message.MessageState = MessageState.Read;
                _context.Update(message);
                await _context.SaveChangesAsync();
            }
            return message;
        }

        public async Task<List<Message>> GetAllMessagesAsync()
        {
            return await _context.Messages.ToListAsync();
        }

        public List<Message> GetAllMessages()
        {
            return _context.Messages.ToList();
        }

        public Boolean MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }

    }
}
