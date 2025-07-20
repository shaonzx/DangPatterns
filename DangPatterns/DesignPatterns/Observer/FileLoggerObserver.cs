namespace DangPatterns.DesignPatterns.Observer
{
    // Concrete Observer 2 - File Logger
    public class FileLoggerObserver : IObserver
    {
        private readonly string _fileName;
        private readonly ILogger<FileLoggerObserver> _logger;

        public FileLoggerObserver(string fileName, ILogger<FileLoggerObserver> logger)
        {
            _fileName = fileName;
            _logger = logger;
        }

        public void Update(string message, DateTime timestamp)
        {
            _logger.LogInformation("📁 FileLoggerObserver triggered - Writing to {FileName} at {Timestamp}. Message: {Message}",
                _fileName, timestamp, message);

            // Simulate file writing delay
            Thread.Sleep(200);

            _logger.LogInformation("✅ Successfully logged to {FileName}", _fileName);
        }
    }
}
