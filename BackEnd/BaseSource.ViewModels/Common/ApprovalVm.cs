using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.Common
{
    /// <summary>
    /// ViewModel để duyệt yêu cầu (Leave/OT)
    /// </summary>
    public class ApproveRequestVm
    {
        [Required(ErrorMessage = "ID yêu cầu là bắt buộc")]
        public string RequestId { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel để từ chối yêu cầu (Leave/OT)
    /// </summary>
    public class RejectRequestVm
    {
        [Required(ErrorMessage = "ID yêu cầu là bắt buộc")]
        public string RequestId { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Lý do từ chối không được vượt quá 500 ký tự")]
        public string? RejectReason { get; set; }
    }
}
