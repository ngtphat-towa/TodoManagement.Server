﻿using System.Security.Claims;

using Application.Interfaces.Services;

using Contracts.Accounts;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Shared.Wrappers;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and account management.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="accountService">The account service for handling user operations.</param>
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Authenticates a user based on credentials.
        /// </summary>
        /// <param name="request">Authentication request data.</param>
        /// <returns>ActionResult with authentication result.</returns>
        [HttpPost("authenticate")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response<AuthenticationResponse>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            var response = await _accountService.AuthenticateAsync(request, GenerateIPAddress());
            return Ok(response);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">Registration request data.</param>
        /// <returns>ActionResult with registration result.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers.Origin;
            var response = await _accountService.RegisterAsync(request, origin!);
            return Ok(response);
        }

        /// <summary>
        /// Confirms the email address associated with a user account.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="code">Confirmation code.</param>
        /// <returns>ActionResult with email confirmation result.</returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            var response = await _accountService.ConfirmEmailAsync(userId, code);
            return Ok(response);
        }

        /// <summary>
        /// Initiates the process to reset a forgotten password.
        /// </summary>
        /// <param name="model">Forgot password request data.</param>
        /// <returns>ActionResult indicating success.</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response<Unit>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _accountService.ForgotPassword(model, Request.Headers.Origin!);
            return Ok(Response<Unit>.Success(Unit.Value, "Password reset instructions sent to your email."));
        }

        /// <summary>
        /// Resets the user's password.
        /// </summary>
        /// <param name="model">Reset password request data.</param>
        /// <returns>ActionResult with password reset result.</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            var response = await _accountService.ResetPassword(model);
            return Ok(response);
        }

        /// <summary>
        /// Logs out the user by invalidating the refresh token and signing them out.
        /// </summary>
        /// <returns>ActionResult indicating the logout status.</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var response = await _accountService.LogoutAsync(userId, GenerateIPAddress());
            return Ok(response);
        }


        /// <summary>
        /// Refreshes the JWT token using the provided refresh token.
        /// </summary>
        /// <param name="model">Refresh token request data.</param>
        /// <returns>ActionResult with new tokens.</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response<AuthenticationResponse>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest tokenRequest)
        {
            // Retrieve the refresh token from the request headers
            var refreshToken = tokenRequest.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is missing or invalid.");
            }
            // Refresh token
            var response = await _accountService.RefreshTokenAsync(refreshToken, GenerateIPAddress());

            return Ok(response);
        }


        /// <summary>
        /// Retrieves the roles assigned to a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>ActionResult with user roles.</returns>
        [HttpGet("roles/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<List<string>>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var response = await _accountService.GetUserRolesAsync(userId);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves the permissions assigned to a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>ActionResult with user permissions.</returns>
        [HttpGet("permissions/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<List<string>>), 200)]
        [ProducesResponseType(typeof(Response<Unit>), 400)]
        public async Task<IActionResult> GetUserPermissions(string userId)
        {
            var response = await _accountService.GetUserPermissionsAsync(userId);
            return Ok(response);
        }

        /// <summary>
        /// Generates the IP address of the client making the request.
        /// </summary>
        /// <returns>Client's IP address as a string.</returns>
        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "Unknown";
            }
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
        }
    }
}