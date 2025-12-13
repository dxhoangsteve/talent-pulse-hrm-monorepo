namespace BaseSource.ViewModels.User
{
    public class UserVm
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public IList<string> Roles { get; set; }
        public bool IsActive { get; set; }
    }
}
