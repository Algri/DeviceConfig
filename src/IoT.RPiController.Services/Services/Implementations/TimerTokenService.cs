using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IoT.RPiController.Services.Services.Implementations;

public class TimerTokenService(IMemoryCache memoryCache, ILogger<TimerTokenService> logger) : ITimerTokenService
{
    public CancellationToken GetCancellationToken(int address, int relayNumber) =>
        memoryCache.TryGetValue(GetCacheKey(address, relayNumber), out CancellationTokenSource cancellationTokenSource)
            ? cancellationTokenSource.Token
            : CancellationToken.None;

    public CancellationToken SetCancellationToken(int address, int relayNumber, CancellationToken cancellationToken)
    {
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        memoryCache.Set(GetCacheKey(address, relayNumber), cancellationTokenSource);

        return cancellationTokenSource.Token;
    }

    public CancellationToken GetOrSetCancellationToken(int address, int relayNumber)
    {
        var cancellationToken = GetCancellationToken(address, relayNumber);

        if (cancellationToken == CancellationToken.None)
        {
            cancellationToken = SetCancellationToken(address, relayNumber, cancellationToken);
        }

        return cancellationToken;
    }

    public void CancelAndRemoveTimer(int address, int relayNumber)
    {
        if (!memoryCache.TryGetValue(GetCacheKey(address, relayNumber), out CancellationTokenSource cancellationTokenSource))
        {
            var msg = $"No cancellation token found for Address: {address}, Relay number: {relayNumber}.";
            logger.LogError(msg);
            throw new ArgumentException(msg);
        }

        cancellationTokenSource.Cancel();

        memoryCache.Remove(GetCacheKey(address, relayNumber));
    }
    
    private static string GetCacheKey(int address, int relayNumber)
    {
        return $"{address}_{relayNumber}_tv";
    }
}