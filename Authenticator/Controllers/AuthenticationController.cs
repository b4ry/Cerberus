using Authenticator.DTOs;
using Authenticator.Generators;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(ILogger<AuthenticationController> logger, ISecurityTokenGenerator securityTokenGenerator) : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger = logger;
        private readonly ISecurityTokenGenerator _securityTokenGenerator = securityTokenGenerator;

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            _logger.LogInformation($"Logging in user {request.UserName}");
            // login process

            string jwt = _securityTokenGenerator.GenerateSecurityToken();

            return Ok(jwt);
        }
    }
}
