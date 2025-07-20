namespace DangPatterns.DesignPatterns.Factory
{
    public interface IEmployee
    {
        void Work();
        void CalculatePay();
        string GetRole();
    }

    // Concrete products
    public class Manager : IEmployee
    {
        public void Work() => Console.WriteLine("Managing team and projects");
        public void CalculatePay() => Console.WriteLine("Calculating salary + bonus");
        public string GetRole() => "Manager";
    }

    public class Developer : IEmployee
    {
        public void Work() => Console.WriteLine("Writing code and debugging");
        public void CalculatePay() => Console.WriteLine("Calculating salary based on hours");
        public string GetRole() => "Developer";
    }

    public class SalesRepresentative : IEmployee
    {
        public void Work() => Console.WriteLine("Meeting clients and closing deals");
        public void CalculatePay() => Console.WriteLine("Calculating base salary + commission");
        public string GetRole() => "Sales Representative";
    }

    public class DeliveryPerson : IEmployee
    {
        public void Work() => Console.WriteLine("Delivering packages to customers");
        public void CalculatePay() => Console.WriteLine("Calculating hourly wage + delivery bonuses");
        public string GetRole() => "Delivery Person";
    }

    // Employee types enumeration
    public enum EmployeeType
    {
        Manager,
        Developer,
        SalesRepresentative,
        DeliveryPerson
    }

}
