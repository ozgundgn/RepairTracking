using System;
using Avalonia.Threading;

namespace RepairTracking.Services;

public class InactivityService
{
    private readonly DispatcherTimer _inactivityTimer;

    // This event will be fired when the user is detected as inactive.
    public event Action? OnInactive;

    /// <summary>
    /// Initializes the inactivity service.
    /// </summary>
    /// <param name="timeout">The period of inactivity before the OnInactive event is fired.</param>
    public InactivityService(TimeSpan timeout)
    {
        // DispatcherTimer runs on the UI thread, which is safe for UI operations.
        _inactivityTimer = new DispatcherTimer
        {
            Interval = timeout
        };
        _inactivityTimer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        // The timer has ticked, meaning the user is inactive.
        Stop(); // Stop the timer
        OnInactive?.Invoke(); // Fire the event
    }

    /// <summary>
    /// Starts the inactivity timer.
    /// </summary>
    public void Start()
    {
        _inactivityTimer.Start();
    }

    /// <summary>
    /// Stops the inactivity timer.
    /// </summary>
    public void Stop()
    {
        _inactivityTimer.Stop();
    }

    /// <summary>
    /// Resets the inactivity timer. This should be called whenever user activity is detected.
    /// </summary>
    public void ResetTimer()
    {
        // This is a common pattern: stop the timer and start it again to reset the countdown.
        _inactivityTimer.Stop();
        _inactivityTimer.Start();
    }
}