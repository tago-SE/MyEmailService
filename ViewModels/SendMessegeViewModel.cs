using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.ViewModels
{
    public class SendMessegeViewModel
    {
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Content { get; set; }

        [Display(Name = "Receiver")]
        public List<SelectListItem> Receivers { get; set; }

        [Required]
        public List<string> SelectedReceivers { get; set; }

        public string ResponseMessege { get; set; }

    }
}
