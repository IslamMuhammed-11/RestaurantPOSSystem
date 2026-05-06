using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs;
using Contracts.DTOs.AuthDTOs;
using Contracts.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contracts.DTOs.BaseResponse;

namespace API_Layer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            if (!user.IsSuccess || user.Value == null)
            {
                var ip = HttpContext.Connection.RemoteIpAddress;
                _logger.LogWarning("Login Faliure With Username = {username}, IP = {ip}", request.Username, ip);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "Invalid Credentials",
                    Status = 401
                });
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Value.PasswordHash);

            if (!isValidPassword)
            {
                var ip = HttpContext.Connection.RemoteIpAddress;
                _logger.LogWarning("Login Faliure Bad Password With Username = {username}, IP = {ip}", request.Username, ip);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "Invalid Credentials",
                    Status = 401
                });
            }

            var claims = new[]
            {
               new Claim( ClaimTypes.NameIdentifier , user.Value.UserID.ToString()),
               new Claim("username" , user.Value.UserName),
               new Claim(ClaimTypes.Role , user.Value.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    issuer: "RestaurantApi",
                    audience: "RestaurantApiUsers",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: creds
                );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = _GnerateRefreshToken();

            user.Value.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.Value.ExpiresAt = DateTime.Now.AddDays(7);
            user.Value.RevokedAt = null;

            var IsSaved = await _userService.SaveRefreshTokenAsync(user.Value);

            if (!IsSaved.IsSuccess || !IsSaved.Value)
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "Unexpected error occurred while Login",
                    Status = 500
                });

            var response = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return Ok(ApiResponse<TokenResponse>.Success(response, "Login successful"));
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequst request)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            if (!user.IsSuccess || user.Value == null)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                _logger.LogWarning(
                     "Refresh Attempt With a Invalid Username = {username} IP = {ip}"
                     , request.Username, ip);

                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "Authentication is required",
                    Status = 401
                });
            }

            if (user.Value.RevokedAt.HasValue)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                _logger.LogWarning(
                     "Refresh Attempt With a Revoked Token Username {username} IP = {ip}"
                     , request.Username, ip);

                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "Valid refresh token is required",
                    Status = 401
                });
            }

            if (!user.Value.ExpiresAt.HasValue || user.Value.ExpiresAt.Value <= DateTime.UtcNow)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                _logger.LogWarning(
                     "Refresh Attempt With a Expired Token Username = {username} IP = {ip}"
                     , request.Username, ip);

                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "Authentication is required",
                    Status = 401
                });
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.Value.RefreshTokenHash);

            if (!isValid)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                _logger.LogWarning(
                     "Refresh Attempt With Invalid Token Username = {username} IP = {ip}"
                     , request.Username, ip);

                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "Authentication is required",
                    Status = 401
                });
            }

            var claims = new[]
{
               new Claim( ClaimTypes.NameIdentifier , user.Value.UserID.ToString()),
               new Claim("username" , user.Value.UserName),
               new Claim(ClaimTypes.Role , user.Value.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    issuer: "RestaurantApi",
                    audience: "RestaurantApiUsers",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: creds
                );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = _GnerateRefreshToken();

            user.Value.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.Value.ExpiresAt = DateTime.Now.AddDays(7);
            user.Value.RevokedAt = null;

            var isSaved = await _userService.SaveRefreshTokenAsync(user.Value);

            if (!isSaved.IsSuccess || !isSaved.IsSuccess)
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "Unexpected error occurred while Login",
                    Status = 500
                });

            var response = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return Ok(ApiResponse<TokenResponse>.Success(response));
        }

        [HttpPost("logout")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            if (!user.IsSuccess || user.Value == null)
                return Ok();

            bool isValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.Value.RefreshTokenHash);

            if (!isValid)
                return Ok();

            var Revoked = await _userService.RevokeToken(user.Value.UserID);

            if (!Revoked.IsSuccess || !Revoked.Value)
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "Unexpected error occurred while Login",
                    Status = 500
                });

            return Ok();
        }

        private static string _GnerateRefreshToken()
        {
            var bytes = new byte[64];

            var rng = RandomNumberGenerator.Create();

            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }
    }
}