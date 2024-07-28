using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Application.Exceptions;
using Application.Interfaces.Services;

using Domain.Settings;

using Identity.Context;
using Identity.Helpers;
using Identity.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Services
{
    /// <summary>
    /// Interface for token service operations including token generation and validation.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for a given user with optional additional claims.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <param name="optionalClaims">Optional claims to include in the token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the JWT token as a string.</returns>
        Task<string> GenerateToken(ApplicationUser user, List<Claim>? optionalClaims = null);

        /// <summary>
        /// Validates a JWT token and returns the claims principal if the token is valid.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>The claims principal if the token is valid; otherwise, returns null.</returns>
        ClaimsPrincipal? ValidateToken(string token);
    }

    /// <summary>
    /// Service for handling JWT token generation, validation, and refresh token management.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDateTimeService _dateTimeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="jwtSettings">Configuration settings for JWT tokens.</param>
        /// <param name="userManager">User manager for handling user operations.</param>
        /// <param name="dateTimeService">Service for handling date and time operations.</param>
        public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager, IDateTimeService dateTimeService)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _dateTimeService = dateTimeService;
        }

        /// <summary>
        /// Generates a JWT token for the specified user, including optional claims.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <param name="optionalClaims">Additional claims to include in the token, or null if no additional claims.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the JWT token as a string.</returns>
        public async Task<string> GenerateToken(ApplicationUser user, List<Claim>? optionalClaims = null)
        {
            // Retrieve claims and roles for the user
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            // Prepare claims for the JWT token
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!),
                new("ip", IpHelper.GetIpAddress())
            };

            // Add user-specific claims
            claims.AddRange(userClaims);

            // Add any optional claims provided
            if (optionalClaims != null)
            {
                claims.AddRange(optionalClaims);
            }

            // Create symmetric security key and signing credentials
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // Create JWT security token
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: _dateTimeService.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
                signingCredentials: signingCredentials);

            // Return the JWT token as a string
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        /// <summary>
        /// Validates a JWT token and returns the claims principal if the token is valid.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>The claims principal extracted from the token if valid; otherwise, throws an exception.</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            try
            {
                // Define token validation parameters
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero // No clock skew tolerance
                };

                // Validate the token and retrieve the claims principal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Check if the validated token is a JWT and uses HMAC-SHA256 algorithm
                if (!(validatedToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                // Verify token expiration
                if (jwtSecurityToken.ValidTo < _dateTimeService.UtcNow)
                {
                    throw new SecurityTokenExpiredException("Token has expired");
                }

                // Return the validated claims principal
                return principal;
            }
            catch (Exception ex)
            {
                // Throw an ApiException if token validation fails
                throw new ApiException($"Token validation failed: {ex.Message}");
            }
        }
    }
}
