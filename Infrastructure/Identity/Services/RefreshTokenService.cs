using System.Security.Cryptography;

using Application.Interfaces.Services;

using Contracts.Accounts;

using Domain.Settings;

using Identity.Context;
using Identity.Helpers;
using Identity.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Services
{
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Generates a refresh token for the specified user and stores it in the database.
        /// </summary>
        /// <param name="user">The user for whom the refresh token is generated.</param>
        /// <returns>The generated refresh token as a string.</returns>
        Task<string> GenerateRefreshToken(ApplicationUser user);
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, string? ipAddress);

        /// <summary>
        /// Invalidates all refresh tokens associated with the specified user.
        /// </summary>
        /// <param name="user">The user for whom to invalidate refresh tokens.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RevokeRefreshTokenAsync(ApplicationUser user, string ipAddress);
        /// <summary>
        /// Validates if the specified refresh token is valid for the given user.
        /// </summary>
        /// <param name="user">The user for whom to validate the refresh token.</param>
        /// <param name="refreshToken">The refresh token to validate.</param>
        /// <returns>True if the refresh token is valid for the user; otherwise, false.</returns>
        bool ValidateRefreshToken(ApplicationUser user, string refreshToken);
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        /// <summary>
        /// JWT settings for token generation and validation.
        /// </summary>
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// User manager for handling user-related operations.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Database context for accessing the identity-related data.
        /// </summary>
        private readonly IdentityContext _dbContext;

        /// <summary>
        /// Date time service for handling date and time operations.
        /// </summary>
        private readonly IDateTimeService _dateTimeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenService"/> class.
        /// </summary>
        /// <param name="jwtSettings">JWT settings for token generation and validation.</param>
        /// <param name="userManager">User manager for handling user-related operations.</param>
        /// <param name="dbContext">Database context for accessing the identity-related data.</param>
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
        /// <returns>The generated refresh token as a string.</returns>
        public async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            // Generate a new refresh token
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRandomToken(),
                Expires = _dateTimeService.UtcNow.AddDays(7),
                Created = _dateTimeService.UtcNow,
                CreatedByIp = IpHelper.GetIpAddress()
            };

            // Add refresh token to database
            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();

            // Return the generated refresh token
            return refreshToken.Token;
        }

        /// <summary>
        /// Invalidates all refresh tokens associated with the specified user.
        /// </summary>
        /// <param name="user">The user for whom to invalidate refresh tokens.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RevokeRefreshTokenAsync(ApplicationUser user, string ipAddress)
        {
            // Retrieve all refresh tokens for the user from database
            var refreshTokens = await _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id).ToListAsync();

            // Update each refresh token with revocation details
            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.Revoked = _dateTimeService.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
            }

            // Save changes to database
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Validates if the specified refresh token is valid for the given user.
        /// </summary>
        /// <param name="user">The user for whom to validate the refresh token.</param>
        /// <param name="refreshToken">The refresh token to validate.</param>
        /// <returns>True if the refresh token is valid for the user; otherwise, false.</returns>
        public bool ValidateRefreshToken(ApplicationUser user, string refreshToken)
        {
            // Retrieve the refresh token from database for the specified user
            var storedToken = _dbContext.RefreshTokens.SingleOrDefault(rt => rt.UserId == user.Id && rt.Token == refreshToken);

            // Check if the stored token exists and is active
            return storedToken != null && storedToken.IsActive;
        }

        /// <summary>
        /// Retrieves a specific refresh token from the database based on user ID and token.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the refresh token.</param>
        /// <param name="refreshToken">The refresh token to retrieve.</param>
        /// <returns>The refresh token if found; otherwise, null.</returns>
        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, string? ipAddress)
        {
            // Retrieve the refresh token from the database for the specified user and token
            var token = await _dbContext.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == refreshToken || (!string.IsNullOrEmpty(ipAddress)
                                                                         && rt.CreatedByIp.Contains(ipAddress)));

            // Return the token if found; otherwise, return null
            return token;
        }

        /// <summary>
        /// Generates a random token for use as a refresh token.
        /// </summary>
        /// <returns>The generated random token as a string.</returns>
        private static string GenerateRandomToken()
        {
            // Generate a random token using RNGCryptoServiceProvider
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            var randomBytes = new byte[40];
            randomNumberGenerator.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
