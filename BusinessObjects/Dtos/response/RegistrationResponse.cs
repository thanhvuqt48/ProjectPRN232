﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.response
{
    public class RegistrationResponse
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string RedirectUrl { get; set; }
    }
}
