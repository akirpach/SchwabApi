namespace backend.Models
{
    public class TokenRefreshSettings
    {
        /// <summary>
        /// How often to check token expiration (in minutes)
        /// </summary>
        public int CheckIntervalMinutes { get; set; } = 15;

        /// <summary>
        /// Refresh token this many minutes before expiration
        /// </summary>
        public int RefreshBeforeExpiryMinutes { get; set; } = 30;

        /// <summary>
        /// Delay after error before retrying (in minutes)
        /// </summary>
        public int ErrorRetryDelayMinutes { get; set; } = 5;

        /// <summary>
        /// Maximum number of consecutive refresh failures before alerting
        /// </summary>
        public int MaxConsecutiveFailures { get; set; } = 3;
    }
}