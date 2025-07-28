public class Increment
{
    private long currentNumber = 0;
    private bool isIncrementing = true;
    private bool resetCounter = false;
    private readonly object lockObject = new object();
    private bool initialized = false;
    public DateTime lastActivityTimestamp = DateTime.UtcNow;
    public bool isStopped = false;

    public int GetCurrentNumber()
    {
        return (int)(currentNumber);
    }

    public async Task StartIncrementing()
    {
        lock (lockObject)
        {
            if (initialized)
            {
                return; // Already initialized, no need to proceed.
            }

            initialized = true;
        }

        isIncrementing = true;
        //isStopped = false;

        await Task.Run(async () =>
        {
            while (isIncrementing)
            {
                lock (lockObject)
                {
                    if (currentNumber < long.MaxValue)
                        currentNumber++;

                    /* counter - reset */
                    if (resetCounter || currentNumber > long.MaxValue)
                    {
                        currentNumber = 0;
                        resetCounter = false;
                        isIncrementing = false;
                    }
                }

                // Introduce an asynchronous delay of 200 milliseconds
                await Task.Delay(200);
            }
        });
    }

    public void StopIncrementing()
    {
        isIncrementing = false;
        //isStopped = true;
        initialized = false;
    }
    public void StopIncrementingExternal()
    {
        isIncrementing = false;
        isStopped = true;
        initialized = false;
    }

    public void ResetIncrementing()
    {
        resetCounter = true;
        initialized = false;
    }
}