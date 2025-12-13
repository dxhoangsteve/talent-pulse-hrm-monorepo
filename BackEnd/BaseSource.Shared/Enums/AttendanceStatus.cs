namespace BaseSource.Shared.Enums
{
    /// <summary>
    /// Trạng thái chấm công
    /// </summary>
    public enum AttendanceStatus : byte
    {
        /// <summary>Có mặt đầy đủ</summary>
        Present = 0,
        
        /// <summary>Vắng mặt không phép</summary>
        Absent = 1,
        
        /// <summary>Đi muộn</summary>
        Late = 2,
        
        /// <summary>Về sớm</summary>
        EarlyLeave = 3,
        
        /// <summary>Làm nửa ngày</summary>
        HalfDay = 4,
        
        /// <summary>Nghỉ phép</summary>
        OnLeave = 5,
        
        /// <summary>Nghỉ lễ</summary>
        Holiday = 6,
        
        /// <summary>Làm việc từ xa (Work From Home)</summary>
        WorkFromHome = 7
    }
}
