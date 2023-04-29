using System.ComponentModel.DataAnnotations;

namespace College.Areas.Identity.Data
{
    public class StudentsModel
    {
        [Key]
        public int RegNo { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string SchoolName { get; set; }
        [Required]
        public string Percentage12th { get; set; }
        [Required]
        public string Percentage10th { get; set; }
        [Required]
        public string City { get; set; }
        public bool? isFeePaid { get; set; }
        [Required]
        public int YearOfStudy { get; set; }

        public int? Attendance { get; set; }

        public int? CGPA { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string ImageURL { get; set; }

        public DepartmentsModel? department_id { get; set; }

        [Required]
        public AspNetUsers user_id { get; set; }
    }
}
