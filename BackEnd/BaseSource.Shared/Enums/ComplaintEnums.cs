namespace BaseSource.Shared.Enums
{
    /// <summary>
    /// Loại khiếu nại lương
    /// </summary>
    public enum ComplaintType
    {
        NotPaid = 0,      // Chưa nhận được lương
        WrongAmount = 1,  // Sai số tiền
        MissingOT = 2,    // Thiếu tiền OT
        MissingBonus = 3, // Thiếu tiền thưởng
        Other = 99        // Khác
    }

    /// <summary>
    /// Trạng thái xử lý khiếu nại
    /// </summary>
    public enum ComplaintStatus
    {
        Pending = 0,   // Chờ xử lý
        InProgress = 1, // Đang xử lý
        Resolved = 2,  // Đã xử lý
        Rejected = 3   // Từ chối
    }
}
