using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeAPITest.Shared.Models
{
    public class LoginRequest : ILoginRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
