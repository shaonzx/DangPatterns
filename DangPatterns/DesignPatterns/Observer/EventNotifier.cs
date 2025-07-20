namespace DangPatterns.DesignPatterns.Observer
{
    // Observer interface - defines the contract for all observers
    public interface IObserver
    {
        void Update(string message, DateTime timestamp);
    }

    // Subject interface - defines the contract for subjects that can be observed
    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void NotifyObservers(string message);
    }

    // Concrete Subject - The Notifier
    public class EventNotifier : ISubject
    {
        private readonly List<IObserver> _observers;
        private readonly string _name;

        public EventNotifier(string name)
        {
            _observers = new List<IObserver>();
            _name = name;
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
            Console.WriteLine($"Observer attached to {_name}. Total observers: {_observers.Count}");
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
            Console.WriteLine($"Observer detached from {_name}. Total observers: {_observers.Count}");
        }

        public void NotifyObservers(string message)
        {
            var timestamp = DateTime.Now;
            Console.WriteLine($"\n[{timestamp:HH:mm:ss}] {_name} is notifying {_observers.Count} observers...");

            foreach (var observer in _observers)
            {
                observer.Update(message, timestamp);
            }
        }

        // Clean method that does heavy work and automatically notifies
        public async Task DoSomethingHeavyAsync()
        {

            // Your heavy work here - could be database operations, API calls, file processing, etc.
            await Task.Delay(10000); // Simulating 10 seconds of heavy work

            // Once work is done, automatically notify all observers
            NotifyObservers($"Heavy operation completed successfully by {_name}!");
        }
    }
}
