using Cerberus.DTOs;
using Cerberus.Generators;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Cerberus.Controllers
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
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorMessage = new StringBuilder();

                errorMessage.AppendJoin('\n', errors);

                return BadRequest(errorMessage);
            }

            _logger.LogInformation($"Logging in user {request.UserName}");
            // login process

            string jwt = _securityTokenGenerator.GenerateSecurityToken();

            return Ok(jwt);
        }
    }
}
