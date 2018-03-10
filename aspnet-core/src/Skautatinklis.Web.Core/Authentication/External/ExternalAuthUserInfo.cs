using System;

namespace Skautatinklis.Authentication.External
{
    public class ExternalAuthUserInfo
    {
        public string ProviderKey { get; set; }

        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public string EmailAddress { get; set; }

        public string Surname { get; set; }

        public string Provider { get; set; }
    }
}
