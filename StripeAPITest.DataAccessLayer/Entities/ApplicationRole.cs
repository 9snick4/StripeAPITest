using Microsoft.AspNetCore.Identity;

namespace StripeAPITest.DataAccessLayer.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        public bool Active { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
