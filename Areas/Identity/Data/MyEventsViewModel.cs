using System.Drawing;

namespace College.Areas.Identity.Data
{
    public class MyEventsViewModel
    {

        public string student_name { get; set; }
        public int event_id { get; set; }
        public int joined_events_id { get; set; }

        public int reg_no { get; set; }
        public string event_name { get; set;}
        public string event_type { get; set;}
        public string event_venue { get; set;}
        public string event_description { get; set; }
        public DateTime event_date { get; set; }

    }

}
