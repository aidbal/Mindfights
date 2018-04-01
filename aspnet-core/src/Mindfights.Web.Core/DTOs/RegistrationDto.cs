using System;

namespace Mindfights.DTOs
{
    public class RegistrationDto
    {
        public long MindfightId;
        public string MindfightName;
        public DateTime MindfightStartTime;
        public DateTime CreationTime;
        public long TeamId;
        public string TeamName;
        public bool IsConfirmed;
    }
}
