namespace UserService_5002.Request
{
	public class MReq_SignUp
	{
		public string PhoneNumber { get; set; }
		public string Password { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public byte Gender { get; set; }
		public string Address { get; set; }
	}
}
