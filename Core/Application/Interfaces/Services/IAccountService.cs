using Contracts.Accounts;

using Shared.Wrappers;

namespace Application.Interfaces.Services;

public interface IAccountService
{
    Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress);
    Task<Response<string>> RegisterAsync(RegisterRequest request, string origin);
    Task<Response<string>> ConfirmEmailAsync(string userId, string code);
    Task ForgotPassword(ForgotPasswordRequest model, string origin);
    Task<Response<string>> ResetPassword(ResetPasswordRequest model);
    Task<Response<string>> LogoutAsync(string userId);
    Task<Response<List<string>>> GetUserRolesAsync(string userId);
    Task<Response<List<string>>> GetUserPermissionsAsync(string userId);
}
