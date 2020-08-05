using ChatCore.Attributes;
using ChatCore.Languages;
using System.ComponentModel.DataAnnotations;

namespace ChatCore.Enums
{
    public enum SearchByEnum
    {
        [Display(Description = "Name", ResourceType = typeof(lang))]
        Name,
        [Display(Description = "Email", ResourceType = typeof(lang))]
        Email,
        [Display(Description = "Phone", ResourceType = typeof(lang))]
        Phone,
        [Display(Description = "Group", ResourceType = typeof(lang))]
        Group
    }
}
