using Application.Exceptions;
using Application.Interfaces.Services;

using Contracts.Accounts;
using Contracts.Email;

using Domain.Enums;
using Domain.Settings;

using Identity.Helpers;
using Identity.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shared.Wrappers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;
        private readonly IDateTimeService _dateTimeService;

        public AccountService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtSettings> jwtSettings,
            IDateTimeService dateTimeService,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _dateTimeService = dateTimeService;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.Email}.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                request.Password,
                false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.Email}'.");
            }

            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }

            var jwtSecurityToken = await GenerateJWToken(user);
            var response = new AuthenticationResponse
            {
                Id = user.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email!,
                UserName = user.UserName!,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                IsVerified = user.EmailConfirmed,
                RefreshToken = GenerateRefreshToken(ipAddress).Token
            };

            return Response<AuthenticationResponse>.Success(response, $"Authenticated {user.UserName}");
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already taken.");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName
            };

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                throw new ApiException($"Email {request.Email} is already registered.");
            }

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                UserPermissionHelper.InitializePermissions(user, Roles.Basic);
                await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());

                var verificationUri = await SendVerificationEmail(user, origin);
                await _emailService.SendAsync(new EmailRequest
                {
                    From = "mail@quingfa.com",
                    To = user.Email,
                    Body = $"Please confirm your account by visiting this URL {verificationUri}",
                    Subject = "Confirm Registration"
                });

                return Response<string>.Success(user.Id, $"User Registered. Please confirm your account by visiting this URL {verificationUri}");
            }
            else
            {
                throw new ApiException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<Response<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApiException($"User with Id '{userId}' not found.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Response<string>.Success(
                    user.Id,
                    $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
            }
            else
            {
                throw new ApiException($"An error occurred while confirming {user.Email}.");
            }
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null)
            {
                // Always return OK response to prevent email enumeration
                return;
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);
            var route = "api/account/reset-password/";
            var endpointUri = new Uri($"{origin}/{route}");
            var emailRequest = new EmailRequest
            {
                Body = $"You reset token is - {code}",
                To = model.Email,
                Subject = "Reset Password"
            };

            await _emailService.SendAsync(emailRequest);
        }

        public async Task<Response<string>> ResetPassword(ResetPasswordRequest model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null)
            {
                throw new ApiException($"No Accounts Registered with {model.Email}.");
            }

            var result = await _userManager.ResetPasswordAsync(account, model.Token, model.Password);
            if (result.Succeeded)
            {
                return Response<string>.Success(model.Email, $"Password Resetted.");
            }
            else
            {
                throw new ApiException($"Error occurred while resetting the password.");
            }
        }

        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = UserPermissionHelper.GetPermissions(user.Permissions); // Assuming user.Permissions is a list of permission strings

            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            var permissionsClaims = permissions.Select(permissions => new Claim("permissions", permissions)).ToList();

            var claims = new List<Claim>
    {
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Name, user.UserName!),
                new (ClaimTypes.Email, user.Email!),
                new ("uid", user.Id),
                new ("ip", IpHelper.GetIpAddress())
    }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionsClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/account/confirm-email/";
            var endpointUri = new Uri($"{origin}/{route}");
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);

            return verificationUri;
        }

        private static string RandomTokenString()
        {
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            var randomBytes = new byte[40];
            randomNumberGenerator.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
        private static void InitializePermissions(ApplicationUser user, Roles role)
        {
            var permissions = UserPermissionHelper.GetPermissionsForRole(role);
            foreach (var permission in permissions)
            {
                var parts = permission.Split("_");
                if (parts.Length == 2 && Enum.TryParse(parts[0], out ControllerPermission controller) && Enum.TryParse(parts[1], out ActionPermission action))
                {
                    user.AddPermission(controller, action);
                }
            }
        }
    }
}
