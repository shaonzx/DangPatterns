namespace DangPatterns.DesignPatterns.Observer
{
    // Concrete Observer 1 - Email Logger with ILogger
    public class EmailLoggerObserver : IObserver
    {
        private readonly string _emailAddress;
        private readonly ILogger<EmailLoggerObserver> _logger;

        public EmailLoggerObserver(string emailAddress, ILogger<EmailLoggerObserver> logger)
        {
            _emailAddress = emailAddress;
            _logger = logger;
        }

        public void Update(string message, DateTime timestamp)
        {
            _logger.LogInformation("📧 EmailLoggerObserver triggered - Sending email to {EmailAddress} at {Timestamp}. Message: {Message}",
                _emailAddress, timestamp, message);

            // Simulate email sending delay
            Thread.Sleep(500);

            _logger.LogInformation("✅ Email sent successfully to {EmailAddress}", _emailAddress);
        }
    }
}
