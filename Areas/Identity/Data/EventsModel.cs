using System.ComponentModel.DataAnnotations;

namespace College.Areas.Identity.Data
{
    public class EventsModel
    {
        [Key]
        public int EventId { get; set; }
        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventType { get; set; }
        [Required]
        public string EventDescription { get; set; }
        [Required]
        public DateTime EventDate{ get; set; }

        [Required]
        public string venue { get; set; }
    }
}
