namespace APIGateway.Request
{
    public class MRes_Password
    {
        public int? IdBranch { get; set; }
        public int? IdUser { get; set; }
        public string? OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
