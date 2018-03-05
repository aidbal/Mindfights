using Abp.Authorization;
using Skautatinklis.Authorization.Roles;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
