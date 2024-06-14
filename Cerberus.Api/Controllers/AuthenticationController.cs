using Cerberus.Api.DTOs;
using Cerberus.Api.Generators;
using Microsoft.AspNetCore.Mvc;

namespace Cerberus.Api.Controllers
{
    /// <summary>
    /// Authentication controller handling login logic.
    /// </summary>
    /// <param name="logger">The controller information logger</param>
    /// <param name="securityTokenGenerator">Generator responsible for generating a security token</param>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(ILogger<AuthenticationController> logger, ISecurityTokenGenerator securityTokenGenerator) : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger = logger;
        private readonly ISecurityTokenGenerator _securityTokenGenerator = securityTokenGenerator;

        /// <summary>
        /// Returns a JWT, if user provides correct credentials.
        /// </summary>
        /// <param name="request">An object encapsulating UserName and Password fields</param>
        /// <returns>
        ///     JWT token, when login succeesful.
        ///     Otherwise, bad request with corresponding errors</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Authentication
        ///     {
        ///         "userName": "user_name",
        ///         "password": "password"
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Returns a JWT</response>
        /// <response code="400">When either a UserName or Password field is not provided or empty</response>
        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            _logger.LogInformation($"Logging in user {request.UserName}");
            // login process

            string jwt = _securityTokenGenerator.GenerateSecurityToken(request.UserName);

            return Ok(jwt);
        }
    }
}
