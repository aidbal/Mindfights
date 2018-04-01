using System.ComponentModel.DataAnnotations;

namespace Mindfights.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}