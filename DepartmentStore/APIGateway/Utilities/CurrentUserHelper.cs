﻿using APIGateway.Response;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace APIGateway.Utilities
{
    public class CurrentUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public MRes_InfoUser GetCurrentUser()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];

            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "IdUser")?.Value;
            var roleClaim = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var branchIdClaim = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "IdBranch")?.Value;
            var fullNameClaim = _httpContextAccessor.HttpContext.User.Claims
                 .FirstOrDefault(c => c.Type == "Fullname")?.Value;
            var emailClaim = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(m => m.Type == "Email")?.Value;
            var accountClaim = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(m => m.Type == ClaimTypes.Name)?.Value;

            var currentUser = new MRes_InfoUser
            {
                IdRole = roleClaim,
                IdUser = userIdClaim,
                IdBranch = branchIdClaim,
                FullName = fullNameClaim,
                Email = emailClaim,
                AccessToken = token,
                PhoneNumber = accountClaim
			};

            return currentUser;
        }
    }
}
