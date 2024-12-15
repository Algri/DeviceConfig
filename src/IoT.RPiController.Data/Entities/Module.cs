using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Data.Entities
{
    public sealed class Module
    {
        public int Id { get; set; }

        [Required] [Range(32, 39)] public byte Address { get; set; }

        [Required] [Range(0, 255)] public byte PortA { get; set; }

        [Required] [Range(0, 255)] public byte PortB { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string? ModuleType { get; set; } = "RM8";

        [StringLength(25)] public string ModuleName { get; set; } = string.Empty;

        // Navigation property for the related TimerValues
        public ICollection<TimerValue> TimerValues { get; set; } = new List<TimerValue>();

        // Navigation property for the related RelayInfo
        public ICollection<RelayInfo> RelayInfos { get; set; } = new List<RelayInfo>();
    }
}