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
        /// Creates a new user in the database and if it does not exist, returns access token. If the user exists, returns an error with a message.
        /// </summary>
        /// <param name="request">An object encapsulating UserName and Password fields</param>
        /// <returns>
        ///     AuthToken when registered a user
        ///     Bad request, when missing fields
        ///     Conflict, when user exists
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/register
        ///     {
        ///         "username": "testUsername",
        ///         "password": "testPassword"
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">AuthToken encapsulating access and refresh tokens</response>
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
                var authToken = await securityTokenGenerator.GenerateSecurityTokenAsync(request.Username, true);

                return Ok(authToken);
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns an access token, if user provides correct credentials.
        /// </summary>
        /// <param name="request">An object encapsulating UserName and Password fields</param>
        /// <returns>
        ///     AuthToken encapsulating access and refresh tokens when login succeesful
        ///     Bad request with corresponding errors
        ///     Unauthorized when login fails
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/login
        ///     {
        ///         "username": "testUsername",
        ///         "password": "testPassword"
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">AuthToken encapsulating access and refresh tokens</response>
        /// <response code="400">When either a Username or a Password field is not provided or empty</response>
        /// <response code="401">When login fails</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            logger.LogInformation($"Logging in user {request.Username}");

            try
            {
                var loggedIn = await authService.LoginUserAsync(request);

                if (loggedIn)
                {
                    var authToken = await securityTokenGenerator.GenerateSecurityTokenAsync(request.Username, true);

                    return Ok(authToken);
                }

                return Unauthorized();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns AuthToken with newly generated access and old refresh tokens.
        /// </summary>
        /// <param name="request">An object encapsulating refresh token id and username for new access token generation</param>
        /// <returns>
        ///     AuthToken encapsulating access and refresh tokens when login succeesful
        ///     Bad request with corresponding errors
        ///     Unauthorized when login fails
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/refreshtoken
        ///     {
        ///         "refreshTokenId": "testRefreshTokenId",
        ///         "username": "testUsername"
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">AuthToken encapsulating access and refresh tokens</response>
        /// <response code="400">When either a Username or a RefreshTokenId field is not provided or empty</response>
        /// <response code="401">When refresh token does not exist or expired</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            logger.LogInformation($"Refreshing token for user {request.Username}");

            try
            {
                var authToken = await authService.RefreshTokenAsync(request);

                if(authToken == null)
                {
                    return Unauthorized();
                }

                return Ok(authToken);
            }
            catch
            {
                throw;
            }
        }
    }
}
