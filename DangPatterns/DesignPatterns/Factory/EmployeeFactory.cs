namespace DangPatterns.DesignPatterns.Factory
{
    public static class EmployeeFactory
    {
        public static IEmployee CreateEmployee(EmployeeType employeeType)
        {
            return employeeType switch
            {
                EmployeeType.Manager => new Manager(),
                EmployeeType.Developer => new Developer(),
                EmployeeType.SalesRepresentative => new SalesRepresentative(),
                EmployeeType.DeliveryPerson => new DeliveryPerson(),
                _ => throw new ArgumentException($"Unknown employee type: {employeeType}")
            };
        }

        // Alternative method using string parameter
        public static IEmployee CreateEmployee(string employeeType)
        {
            return employeeType.ToLower() switch
            {
                "manager" => new Manager(),
                "developer" => new Developer(),
                "salesrepresentative" or "sales representative" or "sales" => new SalesRepresentative(),
                "deliveryperson" or "delivery person" or "delivery" => new DeliveryPerson(),
                _ => throw new ArgumentException($"Unknown employee type: {employeeType}")
            };
        }
    }
}
