using Cerberus.Api.DTOs;
using Cerberus.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cerberus.Api.Controllers
{
    /// <summary>
    /// Controller handling auth logic.
    /// </summary>
    /// <param name="logger">The controller information logger</param>
    /// <param name="securityTokenGenerator">Service generating a security token</param>
    /// <param name="authService">Service managing auth</param>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController(
        ILogger<AuthController> logger,
        ISecurityTokenGenerator securityTokenGenerator,
        IAuthService authService) : ControllerBase
    {
        /// <summary>
        /// Creates a new user in the database. If the user exists, returns conflict with a message.
        /// </summary>
        /// <param name="request">An object encapsulating UserName and Password fields</param>
        /// <returns>
        ///     No content, when registered a user
        ///     Bad request, when missing fields
        ///     Conflict, when user exists</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/user/register
        ///     {
        ///         "username": "testUsername",
        ///         "password": "testPassword"
        ///     }
        /// 
        /// </remarks>
        /// <response code="204">When registered a user</response>
        /// <response code="400">When either a Username or a Password field is not provided or empty</response>
        /// <response code="409">When user exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            logger.LogInformation($"Registering user {request.Username}");

            try
            {
                await authService.RegisterUserAsync(request);
            }
            catch(Exception)
            {
                throw;
            }
            
            return NoContent();
        }

        /// <summary>
        /// Returns a JWT, if user provides correct credentials.
        /// </summary>
        /// <param name="request">An object encapsulating UserName and Password fields</param>
        /// <returns>
        ///     JWT token, when login succeesful.
        ///     Bad request with corresponding errors</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/user/login
        ///     {
        ///         "username": "testUsername",
        ///         "password": "testPassword"
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Returns a JWT</response>
        /// <response code="400">When either a Username or a Password field is not provided or empty</response>
        /// <response code="401">When login fails</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            logger.LogInformation($"Logging in user {request.Username}");

            var loggedIn = await authService.LoginUserAsync(request);

            if (loggedIn)
            {
                string jwt = securityTokenGenerator.GenerateSecurityToken(request.Username);

                return Ok(jwt);
            }

            return Unauthorized();
        }
    }
}
