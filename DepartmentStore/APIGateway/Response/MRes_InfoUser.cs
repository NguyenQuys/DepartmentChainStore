﻿namespace APIGateway.Response
{
    public class MRes_InfoUser
    {
        public string IdUser { get; set; }
        public string IdRole { get; set; }
        public string? IdBranch { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string AccessToken { get; set; }
        public string PhoneNumber { get; set; }
    }
}


