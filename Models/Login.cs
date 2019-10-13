using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.Models
{
    public class Login
    {
        [Key]
        public int LoginId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; }

        public Login()
        {
            // Empty constructor
        }

        public Login(IdentityUser user)
        {
            Timestamp = DateTime.Now;
            User = user;
            UserId = user.Id;
        }
    }
}
