using System.ComponentModel.DataAnnotations;

namespace College.Areas.Identity.Data
{
    public class SuggestionsModel
    {
        [Key]
        public int SuggestionId { get; set; }
        [Required]
        public string SuggestionMessage { get; set; }
        [Required]
        public string Status { get; set; }

        [Required]
        public StudentsModel Students { get; set; }
    }
}
