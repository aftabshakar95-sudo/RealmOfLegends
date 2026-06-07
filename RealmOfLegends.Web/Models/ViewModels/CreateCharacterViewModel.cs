using System.ComponentModel.DataAnnotations;

namespace RealmOfLegends.Web.Models.ViewModels
{
    public class CreateCharacterViewModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Character name must be between 1 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s_-]*$", ErrorMessage = "Character name can only contain letters, numbers, spaces, underscores, and hyphens")]
        [Display(Name = "Character Name")]
        public string? CharacterName { get; set; }

        [Required(ErrorMessage = "Please select a class")]
        [Display(Name = "Character Class")]
        public string Class { get; set; } = string.Empty;
    }
}
