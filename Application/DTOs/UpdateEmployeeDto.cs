namespace FlexFit.Application.DTOs
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string License { get; set; }
        public int EmployeeType { get; set; }
        public string? Password { get; set; }
    }
}
