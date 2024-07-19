using System.Collections.Concurrent;
using System.Security.Cryptography;

using Identity.Models;

namespace Identity.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Application.Exceptions;
using Application.Interfaces.Services;

using Contracts.Accounts;

using Domain.Settings;

using Identity.Context;
using Identity.Helpers;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Microsoft.IdentityModel.Tokens;

public interface ITokenService
{
    Task<string> GenerateToken(ApplicationUser user, List<Claim>? optionalClaims = null);
    ClaimsPrincipal? ValidateToken(string token);
    Task<string> GenerateRefreshToken(ApplicationUser user);
    Task InvalidateRefreshTokenAsync(ApplicationUser user);
    bool ValidateRefreshToken(ApplicationUser user, string refreshToken);
    bool IsTokenExpired(string token);
}

/// <summary>
/// Service responsible for JWT token generation, validation, and refresh token management.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityContext _dbContext;
    private readonly IDateTimeService _dateTimeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="jwtSettings">JWT settings configured in the application.</param>
    /// <param name="userManager">User manager for managing user-related operations.</param>
    /// <param name="dbContext">Database context for accessing persistent data.</param>
    /// <param name="dateTimeService">Date and time service for handling time-related operations.</param>
    public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager, IdentityContext dbContext, IDateTimeService dateTimeService)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
        _dbContext = dbContext;
        _dateTimeService = dateTimeService;
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <param name="optionalClaims">Optional additional claims to include in the token.</param>
    /// <returns>The generated JWT token as a string.</returns>
    public async Task<string> GenerateToken(ApplicationUser user, List<Claim>? optionalClaims = null)
    {
        // Retrieve user claims and roles
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

        // Add optional claims if provided
        if (optionalClaims != null)
        {
            claims.AddRange(optionalClaims);
        }

        // Create symmetric security key and signing credentials
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        // Create JWT security token with specified claims, issuer, audience, expiry, and signing credentials
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: _dateTimeService.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);

        // Write and return the generated JWT token
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    /// <summary>
    /// Validates a JWT token and returns the principal if valid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The claims principal extracted from the token if valid; otherwise, throws an exception.</returns>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        // Create JWT token handler and retrieve key from settings
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        try
        {
            // Configure token validation parameters
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

            // Validate the token and retrieve principal
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // Ensure the validated token is a JWT and uses HMAC-SHA256 algorithm
            if (!(validatedToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            // Check if token has expired
            if (jwtSecurityToken.ValidTo < _dateTimeService.UtcNow)
            {
                throw new SecurityTokenExpiredException("Token has expired");
            }

            // Return the validated principal
            return principal;
        }
        catch (Exception ex)
        {
            // Throw ApiException if token validation fails
            throw new ApiException($"Token validation failed: {ex.Message}");
        }
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
            Expires = _dateTimeService.UtcNow.AddDays(7), // Token expiration set to 7 days
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
    public async Task InvalidateRefreshTokenAsync(ApplicationUser user)
    {
        // Retrieve all refresh tokens for the user from database
        var refreshTokens = await _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id).ToListAsync();

        // Update each refresh token with revocation details
        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.Revoked = _dateTimeService.UtcNow;
            refreshToken.RevokedByIp = IpHelper.GetIpAddress();
        }

        // Save changes to database
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if the specified JWT token is expired.
    /// </summary>
    /// <param name="token">The JWT token to check.</param>
    /// <returns>True if the token is expired; otherwise, false.</returns>
    public bool IsTokenExpired(string token)
    {
        // Create JWT token handler and retrieve key from settings
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        try
        {
            // Configure token validation parameters (no validation for issuer, audience)
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

            // Validate the token and retrieve validated token
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            // Check if token is expired based on ValidTo property
            return jwtSecurityToken?.ValidTo < _dateTimeService.UtcNow;
        }
        catch (Exception)
        {
            // If validation fails, consider the token expired
            return true;
        }
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