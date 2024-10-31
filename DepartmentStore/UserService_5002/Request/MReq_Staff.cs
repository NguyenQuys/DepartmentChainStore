namespace UserService_5002.Request
{
    public class MReq_Staff
    {
        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public byte Gender { get; set; }

        public byte RoleId { get; set; }

        public string IdBranch { get; set; }

        public DateTime BeginDate { get; set; }

        public int Salary { get; set; }
    }
}
