namespace IoT.RPiController.Services.Services.Abstractions;

public interface ITimerTokenService 
{
    CancellationToken GetCancellationToken(int address, int relayNumber);
    CancellationToken SetCancellationToken(int address, int relayNumber, CancellationToken cancellationToken);
    CancellationToken GetOrSetCancellationToken(int address, int relayNumber);
    void CancelAndRemoveTimer(int address, int relayNumber);
}