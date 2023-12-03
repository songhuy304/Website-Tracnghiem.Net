﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCs.Models.Viewmodel
{
    public class UserWithRolesViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}