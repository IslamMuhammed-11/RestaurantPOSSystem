using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.AuthDTOs;
using Contracts.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API_Layer.Controllers
{
    [Route("api/auth/")]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                var ip = HttpContext.Connection.RemoteIpAddress;
                _logger.LogWarning("Login Faliure With Username = {username}, IP = {ip}", request.Username, ip);
                return Unauthorized("Invalid Credentials");
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                var ip = HttpContext.Connection.RemoteIpAddress;
                _logger.LogWarning("Login Faliure Bad Password With Username = {username}, IP = {ip}", request.Username, ip);
                return Unauthorized("Invalid Credentials");
            }

            var claims = new[]
            {
               new Claim( ClaimTypes.NameIdentifier , user.UserID.ToString()),
               new Claim("username" , user.UserName),
               new Claim(ClaimTypes.Role , user.Role)
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

            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.ExpiresAt = DateTime.Now.AddDays(7);
            user.RevokedAt = null;

            bool IsSaved = await _userService.SaveRefreshTokenAsync(user);

            if (!IsSaved)
                return StatusCode(500, "Error Occured While Loggin in");

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequst request)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                _logger.LogWarning(
                     "Refresh Attempt With a Invalid Username = {username} IP = {ip}"
                     , request.Username, ip);

                return Unauthorized("Invalid Request");
            }

            if (user.RevokedAt.HasValue)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                _logger.LogWarning(
                     "Refresh Attempt With a Revoked Token Username {username} IP = {ip}"
                     , request.Username, ip);

                return Unauthorized("Refresh token is revoked");
            }

            if (!user.ExpiresAt.HasValue || user.ExpiresAt.Value <= DateTime.UtcNow)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                _logger.LogWarning(
                     "Refresh Attempt With a Expired Token Username = {username} IP = {ip}"
                     , request.Username, ip);

                return Unauthorized("Refresh token is Expired");
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash);

            if (!isValid)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                _logger.LogWarning(
                     "Refresh Attempt With Invalid Token Username = {username} IP = {ip}"
                     , request.Username, ip);

                return Unauthorized("Invalid refresh token");
            }

            var claims = new[]
{
               new Claim( ClaimTypes.NameIdentifier , user.UserID.ToString()),
               new Claim("username" , user.UserName),
               new Claim(ClaimTypes.Role , user.Role)
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

            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.ExpiresAt = DateTime.Now.AddDays(7);
            user.RevokedAt = null;

            bool isSaved = await _userService.SaveRefreshTokenAsync(user);

            if (!isSaved)
                return StatusCode(500, "Unexpected error occuerd");

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            if (user == null)
                return Ok();

            bool isValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash);

            if (!isValid)
                return Ok();

            bool Revoked = await _userService.RevokeToken(user.UserID);

            if (!Revoked)
                return StatusCode(500, "Unexpected error occured");

            return Ok("Logged out successfully");
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