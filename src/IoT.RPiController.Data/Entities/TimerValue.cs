using IoT.RPiController.Data.Enums;
namespace IoT.RPiController.Data.Entities;

public sealed class TimerValue
{
    public int Id { get; set; }
    public int RelayNumber { get; set; } // Relay number (1 to n)
    public int Timer { get; set; } // Timer value associated with the relay
    public TimerActionEnum TimerAction { get; set; }

    // Foreign key property
    public int ModuleId { get; set; }

    // Navigation property for the related Module
    public Module Module { get; set; }
}