using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Areas.Identity.Data
{
    public class JoinedEventsModel
    {
        [Key]
        public int JoinedEventsId { get; set; }

        [ForeignKey("students")]
        public int reg_no { get; set; }
        
        [ForeignKey("events")]
        public int event_id { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
