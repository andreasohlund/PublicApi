namespace PublicAPI.Tests
{
    using Microsoft.Azure.WebJobs.Extensions.Timers;
    using System;

    class FakeTimerSchedule : TimerSchedule
    {
        public override DateTime GetNextOccurrence(DateTime now)
        {
            return now + TimeSpan.FromSeconds(60);
        }
    }
}
