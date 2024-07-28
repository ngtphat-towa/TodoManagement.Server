using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Application.Interfaces.Services;

using Contracts.Accounts;

using Domain.Settings;

using Identity.Context;
using Identity.Helpers;
using Identity.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Services
{
    /// <summary>
    /// Provides services for managing refresh tokens.
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Generates a refresh token for the specified user and stores it in the database.
        /// </summary>
        /// <param name="user">The user for whom the refresh token is generated.</param>
        /// <returns>The generated refresh token as a JWT.</returns>
        Task<string> GenerateRefreshToken(ApplicationUser user);

        /// <summary>
        /// Retrieves a specific refresh token from the database based on the token and optional IP address.
        /// </summary>
        /// <param name="refreshToken">The refresh token to retrieve.</param>
        /// <param name="ipAddress">The optional IP address associated with the token.</param>
        /// <returns>The refresh token if found; otherwise, null.</returns>
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, string? ipAddress);

        /// <summary>
        /// Invalidates all refresh tokens associated with the specified user.
        /// </summary>
        /// <param name="user">The user for whom to invalidate refresh tokens.</param>
        /// <param name="ipAddress">The IP address from which the revocation request originated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RevokeRefreshTokenAsync(ApplicationUser user, string ipAddress);

        /// <summary>
        /// Validates if the specified JWT refresh token is valid.
        /// </summary>
        /// <param name="jwtRefreshToken">The JWT refresh token to validate.</param>
        /// <returns>True if the refresh token is valid; otherwise, false.</returns>
        string? ValidateRefreshToken(string jwtRefreshToken);
    }

    /// <summary>
    /// Implementation of the <see cref="IRefreshTokenService"/> interface.
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityContext _dbContext;
        private readonly IDateTimeService _dateTimeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenService"/> class.
        /// </summary>
        /// <param name="jwtSettings">JWT settings for token generation and validation.</param>
        /// <param name="userManager">User manager for handling user-related operations.</param>
        /// <param name="dbContext">Database context for accessing identity-related data.</param>
        /// <param name="dateTimeService">Date time service for handling date and time operations.</param>
        public RefreshTokenService(
            IOptions<JwtSettings> jwtSettings,
            UserManager<ApplicationUser> userManager,
            IdentityContext dbContext,
            IDateTimeService dateTimeService)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _dbContext = dbContext;
            _dateTimeService = dateTimeService;
        }

        /// <summary>
        /// Generates a refresh token for the specified user and stores it in the database.
        /// </summary>
        /// <param name="user">The user for whom the refresh token is generated.</param>
        /// <returns>The generated refresh token as a JWT.</returns>
        public async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            // Generate a random token
            var randomToken = GenerateRandomToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = randomToken,
                Expires = _dateTimeService.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
                Created = _dateTimeService.UtcNow,
                CreatedByIp = IpHelper.GetIpAddress()
            };

            // Save the random token in the database
            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();

            // Generate a JWT to return to the client
            return GenerateJwtToken(randomToken);
        }

        /// <summary>
        /// Invalidates all refresh tokens associated with the specified user.
        /// </summary>
        /// <param name="user">The user for whom to invalidate refresh tokens.</param>
        /// <param name="ipAddress">The IP address from which the revocation request originated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RevokeRefreshTokenAsync(ApplicationUser user, string ipAddress)
        {
            var refreshTokens = await _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id).ToListAsync();
            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.Revoked = _dateTimeService.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
            }
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Validates if the specified JWT refresh token is valid.
        /// </summary>
        /// <param name="jwtRefreshToken">The JWT refresh token to validate.</param>
        /// <returns>True if the refresh token is valid; otherwise, false.</returns>
        public string? ValidateRefreshToken(string jwtRefreshToken)
        {
            var randomToken = DecodeJwtToken(jwtRefreshToken);
            return randomToken;
        }

        /// <summary>
        /// Retrieves a specific refresh token from the database based on the token and optional IP address.
        /// </summary>
        /// <param name="refreshToken">The refresh token to retrieve.</param>
        /// <param name="ipAddress">The optional IP address associated with the token.</param>
        /// <returns>The refresh token if found; otherwise, null.</returns>
        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, string? ipAddress)
        {
            var token = await _dbContext.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == refreshToken || (!string.IsNullOrEmpty(ipAddress) && rt.CreatedByIp.Contains(ipAddress)));
            return token;
        }

        /// <summary>
        /// Generates a random token using RNGCryptoServiceProvider.
        /// </summary>
        /// <returns>The generated random token as a string.</returns>
        private static string GenerateRandomToken()
        {
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            var randomBytes = new byte[40];
            randomNumberGenerator.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        /// <summary>
        /// Generates a JWT containing the specified random token.
        /// </summary>
        /// <param name="randomToken">The random token to include in the JWT.</param>
        /// <returns>The generated JWT as a string.</returns>
        private string GenerateJwtToken(string randomToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("refreshToken", randomToken)
                }),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Decodes the JWT to extract the random token.
        /// </summary>
        /// <param name="jwtToken">The JWT containing the random token.</param>
        /// <returns>The extracted random token.</returns>
        private string DecodeJwtToken(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);
            var refreshTokenClaim = jwtSecurityToken.Claims.First(claim => claim.Type == "refreshToken");
            return refreshTokenClaim.Value;
        }
    }
}
