namespace IoT.RPiController.Services.Models;

public class RelayInfoDto
{
    public int Id { get; set; }

    public string Description { get; set; }

    public int RelayNumber { get; set; }

    // Foreign key property
    public int ModuleId { get; set; }
}