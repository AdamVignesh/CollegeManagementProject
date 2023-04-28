using System.ComponentModel.DataAnnotations;

namespace College.Areas.Identity.Data
{
    public class JoinedEventsModel
    {
        [Key]
        public int JoinedEventsId { get; set; }
        [Required]
        public StudentsModel reg_no { get; set; }
        [Required]
        public EventsModel event_id { get;}
    }
}
