using System;
using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.Department
{
    public class CreateDepartmentRequest
    {
        [Required(ErrorMessage = "Tên phòng ban không được để trống")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Trưởng phòng không được để trống")]
        public string ManagerId { get; set; }

        [Required(ErrorMessage = "Phó phòng không được để trống")]
        public string DeputyId { get; set; }
    }
}
