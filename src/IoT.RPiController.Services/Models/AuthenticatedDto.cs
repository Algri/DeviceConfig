namespace IoT.RPiController.Services.Models;

public class AuthenticatedDto
{
    public int Id { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}