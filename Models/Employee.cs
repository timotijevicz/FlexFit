namespace FlexFit.Models
{
    public class Employee : User
    {
      
        public string License { get; set; }

        public EmployeeType EmployeeType { get; set; }
    }

    public enum EmployeeType
    {
        Instructor,
        Guard
    }
}