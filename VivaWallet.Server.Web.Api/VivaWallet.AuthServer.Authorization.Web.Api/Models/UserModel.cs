﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VivaWallet.AuthServer.Authorization.Web.Api.Models
{
    public class UserModel
    {
        [Required]
        [Display(Name = "UserName")]
        public String UserName { get; set; }

        [Required]
        [Display(Name = "Name")]
        public String Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }
    }
}