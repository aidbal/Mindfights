using Abp.Authorization;
using Mindfights.Authorization.Roles;
using Mindfights.Authorization.Users;

namespace Mindfights.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
