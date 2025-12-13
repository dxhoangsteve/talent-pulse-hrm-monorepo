namespace BaseSource.Shared.Enums
{
    /// <summary>
    /// Cấp bậc nhân viên trong công ty
    /// </summary>
    public enum EmployeeLevel : byte
    {
        /// <summary>Thực tập sinh</summary>
        Intern = 0,
        
        /// <summary>Fresher - Mới ra trường (0-1 năm)</summary>
        Fresher = 1,
        
        /// <summary>Junior (1-2 năm kinh nghiệm)</summary>
        Junior = 2,
        
        /// <summary>Mid-level (2-4 năm kinh nghiệm)</summary>
        Mid = 3,
        
        /// <summary>Senior (4+ năm kinh nghiệm)</summary>
        Senior = 4,
        
        /// <summary>Team Lead</summary>
        Lead = 5,
        
        /// <summary>Manager</summary>
        Manager = 6,
        
        /// <summary>Director</summary>
        Director = 7,
        
        /// <summary>C-Level (CEO, CTO, CFO...)</summary>
        CLevel = 8
    }
}
