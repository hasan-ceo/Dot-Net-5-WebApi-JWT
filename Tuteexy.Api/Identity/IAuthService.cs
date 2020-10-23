using System.Threading.Tasks;
using Tuteexy.Models;

namespace Tuteexy.Api.Service
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterUserAsync(RegisterViewModel model);

        Task<AuthResponse> LoginUserAsync(LoginViewModel model);

        Task<AuthResponse> ConfirmEmailAsync(string userId, string token);

        Task<AuthResponse> ForgetPasswordAsync(string email);

        Task<AuthResponse> ResetPasswordAsync(ResetPasswordViewModel model);

        Task<AuthResponse> RefreshToken(AuthResponse model);
    }
}
