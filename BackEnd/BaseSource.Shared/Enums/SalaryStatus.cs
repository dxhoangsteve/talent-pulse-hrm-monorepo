namespace BaseSource.Shared.Enums
{
    /// <summary>
    /// Trạng thái bảng lương
    /// </summary>
    public enum SalaryStatus : byte
    {
        /// <summary>Nháp - Đang tính toán</summary>
        Draft = 0,
        
        /// <summary>Chờ duyệt</summary>
        Pending = 1,
        
        /// <summary>Đã duyệt</summary>
        Approved = 2,
        
        /// <summary>Đã thanh toán</summary>
        Paid = 3,
        
        /// <summary>Đã hủy</summary>
        Cancelled = 4
    }
}
