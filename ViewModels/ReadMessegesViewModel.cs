using Microsoft.AspNetCore.Mvc.Rendering;
using MyEmailService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.ViewModels
{
    public class ReadMessegesViewModel
    {

        [Display(Name = "From")]
        public List<SelectListItem> Senders { get; set; }

        [Display(Name = "From")]
        public string SelectedSender { get; set; }

        public List<string> SelectedSenders { get; set; }

        public List<MessegeViewModel> Messeges { get; set; }

        [Display(Name = "Messeges")]
        public int MessegesCount { get; set; }

        [Display(Name = "Read Messeges")]
        public int ReadMessegesCount { get; set; }

        [Display(Name = "Removed Messeges")]
        public int DeletedMessegesCount { get; set; }
    }
}
