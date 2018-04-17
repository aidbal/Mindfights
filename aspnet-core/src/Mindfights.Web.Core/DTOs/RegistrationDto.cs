using System;
using Abp.AutoMapper;
using Mindfights.Models;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(Registration))]
    public class RegistrationDto
    {
        public long Id;
        public long MindfightId;
        public string MindfightName;
        public DateTime MindfightStartTime;
        public DateTime CreationTime;
        public long TeamId;
        public string TeamName;
        public bool IsConfirmed;
    }
}
