using FlexFit.Models;

namespace FlexFit.Application.DTOs
{
    public class EmployeeRegistrationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string License { get; set; }
        public EmployeeType EmployeeType { get; set; }
    }
}
