namespace FlexFit.Application.DTOs
{
    public class MemberRegistrationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string JMBG { get; set; }
        public string CardNumber { get; set; } // OVO JE KARTICA KOJU JE KUPITI/DOBITI PRED ULAZAK
    }
}
