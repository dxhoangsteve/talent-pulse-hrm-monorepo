using System;

namespace BaseSource.Data.Entities
{
    /// <summary>
    /// Ngày nghỉ lễ
    /// </summary>
    public class Holiday
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>Tên ngày lễ</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>Ngày nghỉ</summary>
        public DateTime Date { get; set; }
        
        /// <summary>Năm</summary>
        public int Year { get; set; }
        
        /// <summary>Ngày lễ hàng năm (lặp lại)</summary>
        public bool IsRecurring { get; set; }
        
        /// <summary>Mô tả</summary>
        public string? Description { get; set; }
        
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
