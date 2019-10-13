using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyEmailService.Data;
using MyEmailService.Handlers;
using MyEmailService.Models;
using MyEmailService.ViewModels;

namespace MyEmailService.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UsersHandler _usersHandler;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MessegesHandler _messagesHandler;

        public HomeController(
            ApplicationDbContext context, 
            ILogger<HomeController> logger, 
            UserManager<IdentityUser> userManager)
        {
            _usersHandler = new UsersHandler(context);
            _userManager = userManager;
            _messagesHandler = new MessegesHandler(context);
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            string username = user.UserName;
            int numUnread = await _messagesHandler.CountUserUnreadMessegesAsync(username);
            UserHomeViewModel vm = new UserHomeViewModel
            {
                Username = user.UserName,
                NumUnreadMessages = numUnread
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ReceivedMessages()
        {
            return Redirect("/Messeges/Received");
        }

        public IActionResult DeliveredMesseges()
        {
            return Redirect("/Messeges/Delivered");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
