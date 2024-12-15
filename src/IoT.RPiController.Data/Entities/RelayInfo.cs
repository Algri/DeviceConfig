using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Data.Entities;

public class RelayInfo
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Description { get; set; }

    [Required]
    public int RelayNumber { get; set; }

    // Foreign key property
    public int ModuleId { get; set; }

    // Navigation property for the parent Module
    public virtual Module Module { get; set; }
}