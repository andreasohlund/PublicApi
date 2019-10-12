namespace PublicAPI.Tests
{
    using Microsoft.Azure.WebJobs.Extensions.Timers;
    using System;

    public partial class IntegrationTests
    {
        class FakeTimerSchedule : TimerSchedule
        {
            public override DateTime GetNextOccurrence(DateTime now)
            {
                return now + TimeSpan.FromSeconds(60);
            }
        }
    }
}
