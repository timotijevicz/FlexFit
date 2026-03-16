namespace FlexFit.Models
{
    public class Employee : User
    {
        public int Id { get; set; }

    

        public string License { get; set; }

        public EmployeeType EmployeeType { get; set; }
    }

    public enum EmployeeType
    {
        Instructor,
        Guard
    }
}