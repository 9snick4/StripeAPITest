using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeAPITest.Shared.Models
{
    public interface ILoginRequest
    {
        string Password { get; set; }
        string Username { get; set; }
    }
}
