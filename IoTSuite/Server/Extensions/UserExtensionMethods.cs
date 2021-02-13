using IoTSuite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTSuite.Server.Extensions
{
    public static class UserExtensionMethods
    {
        public static IEnumerable<BasicAuthUser> WithoutPasswords(this IEnumerable<BasicAuthUser> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static BasicAuthUser WithoutPassword(this BasicAuthUser user)
        {
            return new BasicAuthUser
            {
                Id = user.Id,
                Username = user.Username
            };
        }
    }
}
