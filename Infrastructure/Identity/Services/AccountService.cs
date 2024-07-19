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

using System.Text;

namespace Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ITokenService _tokenService;

        public AccountService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IDateTimeService dateTimeService,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _dateTimeService = dateTimeService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Authenticates a user based on provided credentials (email and password).
        /// </summary>
        /// <param name="request">The authentication request containing email and password.</param>
        /// <param name="ipAddress">The IP address of the client.</param>
        /// <returns>A response containing authentication details.</returns>
        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            // Find the user based on the provided email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // If no user is found with the provided email, throw an exception
                throw new ApiException($"No Accounts Registered with {request.Email}.");
            }

            // Attempt to sign in the user with the provided password
            var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                // If sign-in fails (invalid password), throw an exception
                throw new ApiException($"Invalid Credentials for '{request.Email}'.");
            }

            if (!user.EmailConfirmed)
            {
                // If the user's email is not confirmed, throw an exception
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }

            // Generate a JWT token for the authenticated user
            var token = await _tokenService.GenerateToken(user);

            // Generate a refresh token for the authenticated user
            var refreshToken = await _tokenService.GenerateRefreshToken(user);

            // Prepare an AuthenticationResponse object with user details and tokens
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

            // Return a successful response containing the AuthenticationResponse
            return Response<AuthenticationResponse>.Success(response, $"Authenticated {user.UserName}");
        }

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="request">The registration request containing user details.</param>
        /// <param name="origin">The origin URL for verification.</param>
        /// <returns>A response indicating the registration status.</returns>
        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            // Check if the username is already taken
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already taken.");
            }

            // Adapt RegisterRequest to ApplicationUser and create the user
            var user = request.Adapt<ApplicationUser>();

            // Check if the email is already registered
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                throw new ApiException($"Email {request.Email} is already registered.");
            }

            // Attempt to create the user in the database
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                // Initialize permissions for the new user
                InitializePermissions(user, Roles.Basic);

                // Add the user to the 'Basic' role
                await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());

                // Send a verification email to the user
                var verificationUri = await SendVerificationEmail(user, origin);
                await _emailService.SendAsync(new EmailRequest
                {
                    From = "mail@quingfa.com",
                    To = user.Email!,
                    Body = $"Please confirm your account by visiting this URL {verificationUri}",
                    Subject = "Confirm Registration"
                });

                // Return a successful response indicating the user ID and verification URL
                return Response<string>.Success(user.Id, $"User Registered. Please confirm your account by visiting this URL {verificationUri}");
            }
            else
            {
                // If user creation fails, throw an exception with the error details
                throw new ApiException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        /// <summary>
        /// Confirms the email for a user based on the provided confirmation code.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="code">The confirmation code.</param>
        /// <returns>A response indicating the email confirmation status.</returns>
        public async Task<Response<string>> ConfirmEmailAsync(string userId, string code)
        {
            // Find the user based on the provided user ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // If no user is found with the provided ID, throw an exception
                throw new ApiException($"User with Id '{userId}' not found.");
            }

            // Decode the confirmation code and confirm the user's email
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                // If email confirmation succeeds, return a successful response
                return Response<string>.Success(user.Id, $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
            }
            else
            {
                // If email confirmation fails, throw an exception
                throw new ApiException($"An error occurred while confirming {user.Email}.");
            }
        }

        /// <summary>
        /// Initiates the forgot password process by sending a reset token via email.
        /// </summary>
        /// <param name="model">The forgot password request containing user's email.</param>
        /// <param name="origin">The origin URL for reset password.</param>
        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            // Find the user based on the provided email
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null)
            {
                // If no user is found with the provided email, return early to prevent email enumeration
                return;
            }

            // Generate a password reset token for the user
            var code = await _userManager.GeneratePasswordResetTokenAsync(account);

            // Construct the reset password URL and send it via email
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
        /// <param name="model">The reset password request containing user's email, token, and new password.</param>
        /// <returns>A response indicating the password reset status.</returns>
        public async Task<Response<string>> ResetPassword(ResetPasswordRequest model)
        {
            // Find the user based on the provided email
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null)
            {
                // If no user is found with the provided email, throw an exception
                throw new ApiException($"No Accounts Registered with {model.Email}.");
            }

            // Reset the user's password using the provided reset token and new password
            var result = await _userManager.ResetPasswordAsync(account, model.Token, model.Password);
            if (result.Succeeded)
            {
                // If password reset succeeds, return a successful response
                return Response<string>.Success(model.Email, $"Password Reset.");
            }
            else
            {
                // If password reset fails, throw an exception
                throw new ApiException($"Error occurred while resetting the password.");
            }
        }

        /// <summary>
        /// Logs out the user by invalidating the refresh token and signing them out.
        /// </summary>
        /// <param name="userId">The ID of the user to logout.</param>
        /// <returns>A response indicating the logout status.</returns>
        public async Task<Response<string>> LogoutAsync(string userId)
        {
            // Find the user based on the provided user ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // If no user is found with the provided ID, throw an exception
                throw new ApiException($"User with Id '{userId}' not found.");
            }

            // Invalidate the refresh token for the user
            await _tokenService.InvalidateRefreshTokenAsync(user);

            // Sign out the user from the authentication manager
            await _signInManager.SignOutAsync();

            // Return a successful response indicating the user ID and logout message
            return Response<string>.Success(userId, "Logged out successfully.");
        }

        /// <summary>
        /// Sends a verification email with a confirmation link for the user to confirm their email address.
        /// </summary>
        /// <param name="user">The ApplicationUser to send the verification email to.</param>
        /// <param name="origin">The origin URL for the confirmation link.</param>
        /// <returns>The verification URL sent to the user.</returns>
        private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
        {
            // Generate an email confirmation token for the user
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Encode the confirmation code and construct the verification URL
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/account/confirm-email/";
            var endpointUri = new Uri($"{origin}/{route}");
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);

            // Return the verification URL
            return verificationUri;
        }
        public async Task<Response<List<string>>> GetUserPermissionsAsync(string userId)
        {
            // Find the user based on the provided user ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // If no user is found with the provided ID, throw an exception
                throw new ApiException($"User with Id '{userId}' not found.");
            }

            // Get the permissions stored in the ApplicationUser object
            var permissions = UserPermissionHelper.GetPermissions(user.Permissions);

            // Return a successful response containing the permissions
            return Response<List<string>>.Success(permissions, $"Permissions retrieved for user '{user.UserName}'.");
        }

        public async Task<Response<List<string>>> GetUserRolesAsync(string userId)
        {
            // Find the user based on the provided user ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // If no user is found with the provided ID, throw an exception
                throw new ApiException($"User with Id '{userId}' not found.");
            }

            // Get the roles assigned to the user
            var roles = await _userManager.GetRolesAsync(user);

            // Return a successful response containing the roles
            return Response<List<string>>.Success(roles.ToList(), $"Roles retrieved for user '{user.UserName}'.");
        }

        /// <summary>
        /// Initializes permissions for a user based on the specified role.
        /// </summary>
        /// <param name="user">The ApplicationUser to initialize permissions for.</param>
        /// <param name="role">The role to initialize permissions from.</param>
        private static void InitializePermissions(ApplicationUser user, Roles role)
        {
            // Retrieve permissions for the specified role
            var permissions = UserPermissionHelper.GetPermissionsForRole(role);

            // Add permissions to the user based on the retrieved roles
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
