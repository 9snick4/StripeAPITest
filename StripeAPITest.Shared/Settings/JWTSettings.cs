using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeAPITest.Shared.Settings
{
    public class JwtSettings
    {
        public string SecurityKey { get; init; }

        public string Issuer { get; init; }

        public string Audience { get; init; }

        public int AccessTokenExpirationMinutes { get; init; }

        public int RefreshTokenExpirationMinutes { get; init; }
    }
}
