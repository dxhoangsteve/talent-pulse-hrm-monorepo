using System;

namespace BaseSource.Shared.Helpers
{
    public static class TimeHelper
    {
        // Vietnam timezone: UTC+7
        private static readonly TimeSpan VietnamOffset = TimeSpan.FromHours(7);

        public static DateTime VietnamNow => DateTime.UtcNow.Add(VietnamOffset);
    }
}
