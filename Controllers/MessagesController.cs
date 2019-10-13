using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyEmailService.Data;
using MyEmailService.Handlers;
using MyEmailService.Models;
using MyEmailService.ViewModels;

namespace MyEmailService.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        // SHOULD BE REMOVED IN FUTURE
        private readonly ApplicationDbContext _context;

        private readonly UsersHandler _usersHandler;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MessegesHandler _messagesHandler;

        public MessagesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _usersHandler = new UsersHandler(context);
            _userManager = userManager;
            _messagesHandler = new MessegesHandler(context);
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Messages.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            string username = await GetUserName();
            var message = await _messagesHandler.OpenMessage(id, username);
            if (message == null)
                return NotFound();
            return View(message);
        }

        // GET: Messages/Create
        // Returns a SendMessageViewModel
        public IActionResult Create()
        {
            List<string> names = _usersHandler.GetUserNames();
            var selectionList = new List<SelectListItem>();
            if (names != null)
            {
                foreach (string name in names)
                {
                    selectionList.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            SendMessageViewModel model = new SendMessageViewModel
            {
                Receivers = selectionList,
            };
            return View(model);
        }

        // POST: Messages/Create
        // Sends the message
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SendMessageViewModel model)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            string fromUser = user.UserName;
            string toUser = model.SelectedReceivers.First();
            await _messagesHandler.SendMessageAsync(fromUser, toUser, model.Title, model.Content);
            return RedirectToAction(nameof(Index));
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
                return NotFound();
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,TimeSent,Title,Content,FromUser,ToUser,MessageState")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }

        private async Task<string> GetUserName()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            return user.UserName;
        }
    
    }
}
