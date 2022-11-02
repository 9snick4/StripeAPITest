using Microsoft.AspNetCore.Identity;

namespace StripeAPITest.DataAccessLayer.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RefreshTokenExpirationDate { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }


    }
}
