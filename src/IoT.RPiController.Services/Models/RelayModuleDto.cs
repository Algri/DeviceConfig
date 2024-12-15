namespace IoT.RPiController.Services.Models
{
    public class RelayModuleDto
    {
        public int Id { get; set; }
        public byte Address { get; set; }
        public byte PortA { get; set; }
        public byte PortB { get; set; }
    }
}
