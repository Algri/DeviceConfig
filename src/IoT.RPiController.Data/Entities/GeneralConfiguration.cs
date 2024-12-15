using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Data.Entities;

public class GeneralConfiguration
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Key { get; set; }

    public string Value { get; set; }

    public string Type { get; set; }
}