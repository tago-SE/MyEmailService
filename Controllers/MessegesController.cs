using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyEmailService.Data;
using MyEmailService.Handlers;
using MyEmailService.Models;
using MyEmailService.ViewModels;


namespace MyEmailService.Controllers
{
    [Authorize]
    public class MessegesController : Controller
    {
        private readonly UsersHandler _usersHandler;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MessegesHandler _messegesHandler;

        public MessegesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _usersHandler = new UsersHandler(context);
            _userManager = userManager;
            _messegesHandler = new MessegesHandler(context);
        }

        private async Task<List<SelectListItem>> CreateInboxUserNamesSelectionList()
        {
            var selectionList = new List<SelectListItem>();
            List<string> names = _messegesHandler.GetSenderNamesFromInbox(await GetUserName());
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
            IdentityUser user = await _userManager.GetUserAsync(User);
            string username = user.UserName;
            int messegesCount = await _messegesHandler.CountReceivedUserMessegesAsync(username);
            int readCount = await _usersHandler.GetUserReadMesseges(user);
            int deletedCount = await _usersHandler.GetUserDeletedMesseges(user);
            ReadMessegesViewModel vm = new ReadMessegesViewModel
            {
                Senders = await CreateInboxUserNamesSelectionList(),
                SelectedSenders = new List<string>(),
                MessegesCount = messegesCount,
                ReadMessegesCount = readCount,
                DeletedMessegesCount = deletedCount,
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
                    IdentityUser user = await _userManager.GetUserAsync(User);
                    string username = user.UserName;
                    int messegesCount = await _messegesHandler.CountReceivedUserMessegesAsync(username);
                    string selectedSender = vm.SelectedSenders.First();
                    List<Messege> messeges = await _messegesHandler.GetInboxMessegesFromUser(await GetUserName(), selectedSender);
                    List<SelectListItem> senders = await CreateInboxUserNamesSelectionList();
                    int readCount = await _usersHandler.GetUserReadMesseges(user);
                    int deletedCount = await _usersHandler.GetUserDeletedMesseges(user);
                    vm = new ReadMessegesViewModel
                    {
                        Senders = senders,
                        SelectedSenders = new List<string>(),
                        SelectedSender = selectedSender,
                        Messeges = CreateMessegesViewModel(messeges),
                        MessegesCount = messegesCount,
                        ReadMessegesCount = readCount,
                        DeletedMessegesCount = deletedCount,
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
            return View(await _messegesHandler.GetReceivedMessagesAsync(await GetUserName()));
        }

        // GET: Messeges/Delivered
        public async Task<IActionResult> Delivered()
        {
            return View(await _messegesHandler.GetDeliveredMessagesAsync(await GetUserName()));
        }

        // GET: Messeges/Read/5
        public async Task<IActionResult> Read(int? id)
        {
            if (id == null)
                return NotFound();
            string username = await GetUserName();
            var messege = await _messegesHandler.OpenMessageAsync(id, username);
            if (messege.WasRecentlyRead)
            {
                await _usersHandler.IncreaseUserReadMessegeCount(await _userManager.GetUserAsync(User));
            }
            if (messege == null)
                return NotFound();
            return View(CreateMessegeViewModel(messege));
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
        // Returns a SendMessegeViewModel
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
                Messege model = await _messegesHandler.SendMessageAsync(fromUser, toUser, vm.Title, vm.Content);
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
            var messege = await _messegesHandler.GetMessege(id.HasValue ? id.Value : 0);
            if (messege == null)
            {
                return NotFound();
            }
            return View(CreateMessegeViewModel(messege));
        }

        // POST: Messeges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            await _messegesHandler.DeleteMessegeAsync(id);
            await _usersHandler.IncreaseUserDeletedMessegeCount(user);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> GetUserName()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            return user.UserName;
        }
    }
}
