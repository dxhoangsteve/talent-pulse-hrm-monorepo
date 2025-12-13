namespace BaseSource.Shared.Enums
{
    /// <summary>
    /// Trạng thái của các yêu cầu (nghỉ phép, OT, v.v.)
    /// </summary>
    public enum RequestStatus : byte
    {
        /// <summary>Chờ duyệt</summary>
        Pending = 0,
        
        /// <summary>Đã duyệt</summary>
        Approved = 1,
        
        /// <summary>Từ chối</summary>
        Rejected = 2,
        
        /// <summary>Đã hủy</summary>
        Cancelled = 3
    }
}
