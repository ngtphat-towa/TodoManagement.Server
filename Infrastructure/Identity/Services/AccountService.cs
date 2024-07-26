using System.Text;

using Application.Exceptions;
using Application.Interfaces.Services;

using Contracts.Accounts;
using Contracts.Email;

using Domain.Enums;

using Identity.Models;

using Mapster;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using Shared.Wrappers;

namespace Identity.Services
{
    /// <summary>
    /// Service for handling account-related operations such as user registration, authentication, and password management.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="userManager">The user manager used to manage user accounts.</param>
        /// <param name="roleManager">The role manager used to manage user roles.</param>
        /// <param name="signInManager">The sign-in manager used to handle user sign-ins.</param>
        /// <param name="emailService">The email service used for sending emails.</param>
        /// <param name="tokenService">The token service used for generating JWT tokens.</param>
        /// <param name="refreshTokenService">The refresh token service used for managing refresh tokens.</param>
        public AccountService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
        }

        /// <summary>
        /// Authenticates a user using the provided credentials (email and password).
        /// </summary>
        /// <param name="request">The authentication request containing email and password.</param>
        /// <param name="ipAddress">The IP address of the client making the request.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AuthenticationResponse"/> with authentication details.</returns>
        /// <exception cref="ApiException">Thrown when the user is not found, credentials are invalid, or email is not confirmed.</exception>
        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.Email}.");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.Email}'.");
            }

            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }

            var token = await _tokenService.GenerateToken(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user);

            var response = new AuthenticationResponse
            {
                Id = user.Id,
                JWToken = token,
                Email = user.Email!,
                UserName = user.UserName!,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                IsVerified = user.EmailConfirmed,
                RefreshToken = refreshToken
            };

            return Response<AuthenticationResponse>.Success(response, $"Authenticated {user.UserName}");
        }

        /// <summary>
        /// Registers a new user with the provided details and sends a verification email.
        /// </summary>
        /// <param name="request">The registration request containing user details.</param>
        /// <param name="origin">The origin URL used for generating verification links.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> indicating the registration status.</returns>
        /// <exception cref="ApiException">Thrown when the username or email is already taken, or when user creation fails.</exception>
        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already taken.");
            }

            var user = request.Adapt<ApplicationUser>();

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                throw new ApiException($"Email {request.Email} is already registered.");
            }

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                InitializePermissions(user, Roles.Basic);
                await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());

                var verificationUri = await SendVerificationEmail(user, origin);
                await _emailService.SendAsync(new EmailRequest
                {
                    From = "mail@quingfa.com",
                    To = user.Email!,
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

        /// <summary>
        /// Confirms the user's email using the provided confirmation code.
        /// </summary>
        /// <param name="userId">The ID of the user whose email is to be confirmed.</param>
        /// <param name="code">The confirmation code.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> indicating the email confirmation status.</returns>
        /// <exception cref="ApiException">Thrown when the user is not found or email confirmation fails.</exception>
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
                return Response<string>.Success(user.Id, $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
            }
            else
            {
                throw new ApiException($"An error occurred while confirming {user.Email}.");
            }
        }

        /// <summary>
        /// Initiates the forgot password process by sending a reset token via email.
        /// </summary>
        /// <param name="model">The forgot password request containing the user's email.</param>
        /// <param name="origin">The origin URL used for generating the reset password link.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null)
            {
                return; // To prevent email enumeration, do nothing if the account is not found
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);
            var route = "api/account/reset-password/";
            var endpointUri = new Uri($"{origin}/{route}");
            var emailRequest = new EmailRequest
            {
                Body = $"Your reset token is - {code}",
                To = model.Email,
                Subject = "Reset Password"
            };

            await _emailService.SendAsync(emailRequest);
        }

        /// <summary>
        /// Resets the user's password using the provided reset password token.
        /// </summary>
        /// <param name="model">The reset password request containing the user's email, token, and new password.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> indicating the password reset status.</returns>
        /// <exception cref="ApiException">Thrown when the user is not found or password reset fails.</exception>
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
                return Response<string>.Success(model.Email, $"Password Reset.");
            }
            else
            {
                throw new ApiException($"Error occurred while resetting the password.");
            }
        }

        /// <summary>
        /// Logs out the user and revokes their refresh token.
        /// </summary>
        /// <param name="userId">The ID of the user to log out.</param>
        /// <param name="ipAddress">The IP address of the client making the request.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> indicating the logout status.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        public async Task<Response<string>> LogoutAsync(string userId, string ipAddress)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            await _refreshTokenService.RevokeRefreshTokenAsync(user, ipAddress);
            await _signInManager.SignOutAsync();

            return Response<string>.Success(user.Id, "Logged out successfully.");
        }

        /// <summary>
        /// Sends a verification email to the user with a link to confirm their email address.
        /// </summary>
        /// <param name="user">The user to send the verification email to.</param>
        /// <param name="origin">The origin URL used for generating the verification link.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the verification URL.</returns>
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

        /// <summary>
        /// Retrieves the permissions assigned to a user.
        /// </summary>
        /// <param name="userId">The ID of the user whose permissions are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> with the list of permissions.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        public async Task<Response<List<string>>> GetUserPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with Id '{userId}' not found.");
            }

            var permissions = UserPermissionHelper.GetPermissions(user.Permissions);

            return Response<List<string>>.Success(permissions, $"Permissions retrieved for user '{user.UserName}'.");
        }

        /// <summary>
        /// Retrieves the roles assigned to a user.
        /// </summary>
        /// <param name="userId">The ID of the user whose roles are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> with the list of roles.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        public async Task<Response<List<string>>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with Id '{userId}' not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Response<List<string>>.Success(roles.ToList(), $"Roles retrieved for user '{user.UserName}'.");
        }

        /// <summary>
        /// Initializes permissions for a user based on their role.
        /// </summary>
        /// <param name="user">The user to initialize permissions for.</param>
        /// <param name="role">The role to initialize permissions from.</param>
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

        /// <summary>
        /// Refreshes the JWT token using the provided refresh token.
        /// </summary>
        /// <param name="token">The refresh token.</param>
        /// <param name="ipAddress">The IP address of the client making the request.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AuthenticationResponse"/> with new authentication details.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the refresh token is invalid, inactive, or when the associated user is not found.</exception>
        public async Task<Response<AuthenticationResponse>> RefreshTokenAsync(string token, string ipAddress)
        {
            var existingRefreshToken = await _refreshTokenService.GetRefreshTokenAsync(token, ipAddress);
            if (existingRefreshToken == null || !existingRefreshToken.IsActive)
            {
                throw new KeyNotFoundException("Invalid or inactive refresh token.");
            }

            var user = await _userManager.FindByIdAsync(existingRefreshToken.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User associated with the refresh token not found.");
            }

            await _refreshTokenService.RevokeRefreshTokenAsync(user, ipAddress);

            var newToken = await _tokenService.GenerateToken(user);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(user);

            var response = new AuthenticationResponse
            {
                Id = user.Id,
                JWToken = newToken,
                Email = user.Email!,
                UserName = user.UserName!,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                IsVerified = user.EmailConfirmed,
                RefreshToken = newRefreshToken
            };

            return Response<AuthenticationResponse>.Success(response, "Token refreshed successfully.");
        }
    }
}
