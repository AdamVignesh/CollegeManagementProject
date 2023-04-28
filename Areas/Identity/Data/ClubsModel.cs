using System.ComponentModel.DataAnnotations;

namespace College.Areas.Identity.Data
{
    public class ClubsModel
    {
        [Key]
        public int ClubId { get;set;}

        [Required]
        public string ClubName { get;set;}

        [Required]
        public string ClubDescription { get;set;}
    }

}
