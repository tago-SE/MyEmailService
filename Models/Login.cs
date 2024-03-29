﻿using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEmailService.Models
{
    public class Login
    {
        [Key]
        public int LoginId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual IdentityUser User { get; set; }

        public Login()
        {
            // Required empty constructor
        }

        public Login(IdentityUser user)
        {
            Timestamp = DateTime.Now;
            User = user;
            UserId = user.Id;
        }
    }
}
