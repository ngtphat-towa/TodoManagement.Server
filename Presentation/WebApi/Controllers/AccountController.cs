using Application.Interfaces.Services;

using Contracts.Accounts;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and account management.
    /// </summary>
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
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _accountService.AuthenticateAsync(request, GenerateIPAddress()));
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">Registration request data.</param>
        /// <returns>ActionResult with registration result.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers.Origin;
            return Ok(await _accountService.RegisterAsync(request, origin!));
        }

        /// <summary>
        /// Confirms the email address associated with a user account.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="code">Confirmation code.</param>
        /// <returns>ActionResult with email confirmation result.</returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            var origin = Request.Headers.Origin;
            return Ok(await _accountService.ConfirmEmailAsync(userId, code));
        }

        /// <summary>
        /// Initiates the process to reset a forgotten password.
        /// </summary>
        /// <param name="model">Forgot password request data.</param>
        /// <returns>ActionResult indicating success.</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _accountService.ForgotPassword(model, Request.Headers.Origin!);
            return Ok();
        }

        /// <summary>
        /// Resets the user's password.
        /// </summary>
        /// <param name="model">Reset password request data.</param>
        /// <returns>ActionResult with password reset result.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            return Ok(await _accountService.ResetPassword(model));
        }

        /// <summary>
        /// Generates the IP address of the client making the request.
        /// </summary>
        /// <returns>Client's IP address as a string.</returns>
        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"]!;
            else
                return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        }
    }
}
