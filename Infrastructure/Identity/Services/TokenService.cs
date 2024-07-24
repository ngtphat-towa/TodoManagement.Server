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


namespace Identity.Services;
public interface ITokenService
{
    Task<string> GenerateToken(ApplicationUser user, List<Claim>? optionalClaims = null);
    ClaimsPrincipal? ValidateToken(string token);
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
}