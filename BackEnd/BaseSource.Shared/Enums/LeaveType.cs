namespace BaseSource.Shared.Enums
{
    /// <summary>
    /// Loại nghỉ phép
    /// </summary>
    public enum LeaveType : byte
    {
        /// <summary>Nghỉ phép năm</summary>
        Annual = 0,
        
        /// <summary>Nghỉ ốm</summary>
        Sick = 1,
        
        /// <summary>Nghỉ không lương</summary>
        Unpaid = 2,
        
        /// <summary>Nghỉ thai sản (nữ)</summary>
        Maternity = 3,
        
        /// <summary>Nghỉ thai sản (nam)</summary>
        Paternity = 4,
        
        /// <summary>Nghỉ tang</summary>
        Bereavement = 5,
        
        /// <summary>Nghỉ cưới</summary>
        Wedding = 6,
        
        /// <summary>Nghỉ bù</summary>
        Compensatory = 7,
        
        /// <summary>Khác</summary>
        Other = 99
    }
}
