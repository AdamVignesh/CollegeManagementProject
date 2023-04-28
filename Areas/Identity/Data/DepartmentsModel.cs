using System.ComponentModel.DataAnnotations;

namespace College.Areas.Identity.Data
{
    public class DepartmentsModel
    {
        [Key]
        public int DeptId { get; set; }

        [Required]
        public string DeptName { get; set; }

        [Required]
        public string DeptHead { get; set; }
    }
}
