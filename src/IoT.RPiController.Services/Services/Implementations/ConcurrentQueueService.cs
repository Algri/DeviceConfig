using System.Collections.Concurrent;

namespace IoT.RPiController.Services.Services.Implementations;

public sealed class ConcurrentQueueService
{
    private readonly BlockingCollection<Func<Task>> _queue = new();

    public event EventHandler? TaskEnqueued;
    
    public void EnqueueRequest(Func<Task> requestHandler)
    {
        _queue.Add(requestHandler);
        OnTaskEnqueued(EventArgs.Empty);  // Trigger the event when a new task is added
    }

    private void OnTaskEnqueued(EventArgs e)
    {
        TaskEnqueued?.Invoke(this, e);
    }

    public void StartWorker(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var requestHandler = _queue.Take(cancellationToken);
                    if (requestHandler != null)
                    {
                        await requestHandler();
                    }
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                    break;
                }
                catch (Exception ex)
                {
                    // Handle other exceptions as needed
                    Console.WriteLine(ex);
                }
            }
        }, cancellationToken);
    }
}