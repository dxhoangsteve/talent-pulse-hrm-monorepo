namespace BaseSource.Shared.Enums
{
    /// <summary>
    /// Trạng thái làm việc của nhân viên
    /// </summary>
    public enum EmployeeStatus : byte
    {
        /// <summary>Đang làm việc</summary>
        Active = 0,
        
        /// <summary>Đang thử việc</summary>
        Probation = 1,
        
        /// <summary>Đang nghỉ phép dài hạn</summary>
        OnLeave = 2,
        
        /// <summary>Tạm nghỉ việc</summary>
        Inactive = 3,
        
        /// <summary>Đã nghỉ việc</summary>
        Terminated = 4
    }
}
