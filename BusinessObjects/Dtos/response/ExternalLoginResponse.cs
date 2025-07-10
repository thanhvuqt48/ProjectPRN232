using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.response
{
    public class ExternalLoginResponse
    {
        public bool RequiresRegistration { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AuthProviderId { get; set; }
        public string AuthProvider { get; set; }
        public int AccountId { get; set; }
        public string RedirectUrl { get; set; }
    }
}
