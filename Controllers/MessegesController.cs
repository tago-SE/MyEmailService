using System.Collections.Generic;
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
    public class MessegesController : Controller
    {
        // SHOULD BE REMOVED IN FUTURE
        private readonly ApplicationDbContext _context;

        private readonly UsersHandler _usersHandler;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MessegesHandler _messagesHandler;

        public MessegesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _usersHandler = new UsersHandler(context);
            _userManager = userManager;
            _messagesHandler = new MessegesHandler(context);
        }

        // GET: Messeges
        public async Task<IActionResult> Index()
        {
            return View(await _messagesHandler.GetAllMessagesAsync());
        }

        // GET: Messeges/Received
        public async Task<IActionResult> Received()
        {
            return View(await _messagesHandler.GetReceivedMessagesAsync(await GetUserName()));
        }

        // GET: Messeges/Delivered
        public async Task<IActionResult> Delivered()
        {
            return View(await _messagesHandler.GetDeliveredMessagesAsync(await GetUserName()));
        }


        // GET: Messeges/Details/5
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

        private SendMessegeViewModel CreateSendMessegeViewModel()
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
            SendMessegeViewModel vm = new SendMessegeViewModel
            {
                Receivers = selectionList,
            };
            return vm;
        }

        // GET: Messeges/Create
        // Returns a SendMessageViewModel
        public IActionResult Create()
        {
            return View(CreateSendMessegeViewModel());
        }

        // POST: Messeges/Create
        // Sends the message
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SendMessegeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear(); 
                IdentityUser user = await _userManager.GetUserAsync(User);
                string fromUser = user.UserName;
                string toUser = vm.SelectedReceivers.First();
                Messege model = await _messagesHandler.SendMessageAsync(fromUser, toUser, vm.Title, vm.Content);
                vm = CreateSendMessegeViewModel();
                vm.ResponseMessege = "Messege {" + model.MessageId + "} was sent to " + model.ToUser + ", " + model.TimeSent + ".";
            }
            return View(vm);
        }

        // GET: Messeges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var message = await _context.Messeges.FindAsync(id);
            if (message == null)
                return NotFound();
            return View(message);
        }

        // POST: Messeges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,TimeSent,Title,Content,FromUser,ToUser,MessageState")] Messege message)
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

        // GET: Messeges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messeges
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messeges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messeges.FindAsync(id);
            _context.Messeges.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messeges.Any(e => e.MessageId == id);
        }

        private async Task<string> GetUserName()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            return user.UserName;
        }
    }
}
