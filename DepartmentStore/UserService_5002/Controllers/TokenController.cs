//using Microsoft.AspNetCore.Mvc;
//using ProductService_5000.Helper;
//using System.Threading.Tasks;

//public class TokenController : Controller
//{
//    private readonly JwtHelper _tokenService;

//    public TokenController(JwtHelper tokenService)
//    {
//        _tokenService = tokenService;
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetToken()
//    {
//        var token = await _tokenService.GetTokenAsync();
//        return Ok(new { access_token = token });
//    }
//}
