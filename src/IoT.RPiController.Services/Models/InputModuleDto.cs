namespace IoT.RPiController.Services.Models
{
    public class InputModuleDto
    {
        public int Id { get; set; }
        public byte Address { get; set; }
        public byte PortA { get; set; }
        public byte PortB { get; set; }
        public string? ModuleType { get; set; }

        public string? ModuleName { get; set; } = string.Empty;
    }
}