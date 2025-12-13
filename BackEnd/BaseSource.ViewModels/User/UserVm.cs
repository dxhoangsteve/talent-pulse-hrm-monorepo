namespace BaseSource.ViewModels.User
{
    public class UserVm
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; }
        
        // Additional fields for department view
        public string? DepartmentName { get; set; }
        public string? Position { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}

