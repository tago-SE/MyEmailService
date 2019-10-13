using System;
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

        private async Task<List<SelectListItem>> CreateInboxUserNamesSelectionList()
        {
            var selectionList = new List<SelectListItem>();
            List<string> names = _messagesHandler.GetSenderNamesFromInbox(await GetUserName());
            if (names != null)
                foreach (string name in names)
                    selectionList.Add(new SelectListItem { Text = name, Value = name });
            return selectionList;
        }

        private MessegeViewModel CreateMessegeViewModel(Messege model)
        {
            return new MessegeViewModel
            {
                Id = model.MessageId,
                Title = model.Title,
                Content = model.Content,
                TimeSent = model.TimeSent,
                MessegeState = model.MessegeState,
                FromUser = model.FromUser,
                ToUser = model.ToUser,
            };
        }

        private List<MessegeViewModel> CreateMessegesViewModel(List<Messege> models)
        {
            List<MessegeViewModel> viewModels = new List<MessegeViewModel>();
            foreach (Messege model in models)
            {
                viewModels.Add(CreateMessegeViewModel(model));
            }
            return viewModels;
        }

        // Index
        // GET: Messeges
        // Index page acts as the user inbox
        public async Task<IActionResult> Index()
        {
            string username = await GetUserName();
            int messegesCount = await _messagesHandler.CountReceivedUserMessegesAsync(username);
            ReadMessegesViewModel vm = new ReadMessegesViewModel
            {
                Senders = await CreateInboxUserNamesSelectionList(),
                SelectedSenders = new List<string>(),
                MessegesCount = messegesCount,
            };
            return View(vm);
        }

        // Index
        // Post: Messeges
        // Queries sent messeges from a selected user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ReadMessegesViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string username = await GetUserName();
                    int messegesCount = await _messagesHandler.CountReceivedUserMessegesAsync(username);
                    string selectedSender = vm.SelectedSenders.First();
                    List<Messege> messeges = await _messagesHandler.GetInboxMessegesFromUser(await GetUserName(), selectedSender);
                    List<SelectListItem> senders = await CreateInboxUserNamesSelectionList();
                    vm = new ReadMessegesViewModel
                    {
                        Senders = senders,
                        SelectedSenders = new List<string>(),
                        SelectedSender = selectedSender,
                        Messeges = CreateMessegesViewModel(messeges),
                        MessegesCount = messegesCount,
                    };
                    return View(vm);
                } catch (Exception e)
                {
                   // Dummy catch-exception
                }
            }
            return await Index();
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

        // GET: Messeges/Read/5
        public async Task<IActionResult> Read(int? id)
        {
            if (id == null)
                return NotFound();
            string username = await GetUserName();
            var message = await _messagesHandler.OpenMessage(id, username);
            if (message == null)
                return NotFound();
            return View(CreateMessegeViewModel(message));
        }

        private async Task<List<SelectListItem>> CreateUserNamesSelectionList()
        {
            var selectionList = new List<SelectListItem>();
            List<string> names = await _usersHandler.GetUserNames();
            if (names != null)
                foreach (string name in names)
                    selectionList.Add(new SelectListItem { Text = name, Value = name });
            return selectionList;
        }
  
        private async Task<SendMessegeViewModel> CreateSendMessegeViewModel()
        {
            SendMessegeViewModel vm = new SendMessegeViewModel
            {
                Receivers = await CreateUserNamesSelectionList(),
            };
            return vm;
        }

        // GET: Messeges/Create
        // Returns a SendMessageViewModel
        public async Task<IActionResult> Create()
        {
            return View(await CreateSendMessegeViewModel());
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
                vm = await CreateSendMessegeViewModel();
                vm.ResponseMessege = "Messege {" + model.MessageId + "} was sent to " + model.ToUser + ", " + model.TimeSent + ".";
            }
            return View(vm);
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
