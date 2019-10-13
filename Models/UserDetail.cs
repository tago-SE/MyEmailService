using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.Models
{
    public class UserDetail
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public int ReadMessegeCount { get; set; }

        public int DeletedMessegeCount { get; set; }

        public virtual IdentityUser User { get; set; }

        public UserDetail()
        {
            // Required empty constructor
        }

        public UserDetail(IdentityUser user)
        {
            User = user;
            UserId = user.Id;

        }
    }
}
