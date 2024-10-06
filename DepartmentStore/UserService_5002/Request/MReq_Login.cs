namespace UserService_5002.Request
{
    public class MReq_Login
    {
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }

        public string? BranchAccount { get; set; }
        public string? BranchPassword { get; set; }
    }
}
