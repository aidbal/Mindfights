using System.ComponentModel.DataAnnotations;

namespace Skautatinklis.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}