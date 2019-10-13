using MyEmailService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.ViewModels
{
    public class MessegeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Sent")]
        public DateTime TimeSent { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Messege")]
        public string Content { get; set; }

        [Display(Name = "From")]
        public string FromUser { get; set; }

        [Display(Name = "To")]
        public string ToUser { get; set; }

        [Display(Name = "Status")]
        public MessegeState MessegeState { get; set; }

    }
}
