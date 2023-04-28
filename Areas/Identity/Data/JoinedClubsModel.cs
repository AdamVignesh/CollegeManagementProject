using System.ComponentModel.DataAnnotations;

namespace College.Areas.Identity.Data
{
    public class JoinedClubsModel
    {
        [Key]
        public int JoinedClubId { get; set; }

        public bool? CludHead { get; set; }

        [Required]
        public StudentsModel reg_no { get; set; }
        [Required]
        public ClubsModel club_id { get; set; }
    }
}
