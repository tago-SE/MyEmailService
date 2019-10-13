using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.ViewModels
{
    public class UserHomeViewModel
    {
        public string Username { get; set; }

        public DateTime LastLogin { get; set; }

        public int NumLoginsThisMonth { get; set; }

        public int NumUnreadMessages { get; set; }

        public string ReadURL { get; set; }
    }
}
