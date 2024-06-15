using Cerberus.Api.DTOs;
using Cerberus.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cerberus.Api.Controllers
{
    /// <summary>
    /// Authentication controller handling login logic.
    /// </summary>
    /// <param name="logger">The controller information logger</param>
    /// <param name="securityTokenGenerator">Service generating a security token</param>
    /// <param name="userRegistrationService">Service registering a user</param>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController(
        ILogger<AuthenticationController> logger,
        ISecurityTokenGenerator securityTokenGenerator,
        IUserRegistrationService userRegistrationService) : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger = logger;
        private readonly ISecurityTokenGenerator _securityTokenGenerator = securityTokenGenerator;
        private readonly IUserRegistrationService userRegistrationService = userRegistrationService;

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            _logger.LogInformation($"Registering user {request.Username}");

            var userRegistered = await userRegistrationService.RegisterUserAsync(request);

            if(userRegistered)
            {
                return NoContent();
            }

            return Conflict();
        }

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
        ///         "username": "testUsername",
        ///         "password": "testPassword"
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Returns a JWT</response>
        /// <response code="400">When either a Username or a Password field is not provided or empty</response>
        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            _logger.LogInformation($"Logging in user {request.Username}");
            // login process
            string jwt = _securityTokenGenerator.GenerateSecurityToken(request.Username);

            return Ok(jwt);
        }
    }
}
