using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tuteexy.Models;
using Tuteexy.Service;
using Tuteexy.Utility;

namespace Tuteexy.Api.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        private IConfiguration _configuration;
        const int accessTokenExpiryDays = 1;
        const int refreshTokenExpiryDays = 1;

        public AuthService(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateAccessToken(IdentityUser user)
        {
            
            var isUserAdmin = await _userManager.IsInRoleAsync(user,SD.Role_Admin);

            List<Claim> AuthClaims = new List<Claim>() {
            new Claim (JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

            new Claim (JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim (JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),
			
			// Add the ClaimType Role which carries the Role of the user
			new Claim (ClaimTypes.Role, isUserAdmin == true ? SD.Role_Admin : SD.Role_User),
            new Claim("url", isUserAdmin == true ? SD.Role_Admin : SD.Role_User)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: AuthClaims,
                expires: DateTime.Now.AddDays(accessTokenExpiryDays),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
                throw new NullReferenceException("Reigster Model is null");

            if (model.Password != model.ConfirmPassword)
                return new AuthResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Role_User);

                var refreshToken = await _userManager.GenerateConcurrencyStampAsync(user);
                await _unitOfWork.UserRefreshToken.AddAsync(new UserRefreshToken
                {
                    UserID = user.Id,
                    RefreshToken = refreshToken,
                    ExpiryDate = DateTime.Now.AddDays(1)
                });
                _unitOfWork.Save();

                return new AuthResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new AuthResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };

        }

        public async Task<AuthResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new AuthResponse
                {
                    Message = "There is no user with that Email address",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new AuthResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };

            
            // Generate AccessToken
            string tokenAsString =await GenerateAccessToken(user);

            // Generate Refresh Token
            var refreshToken = await _userManager.GenerateConcurrencyStampAsync(user);

            // Save Refresh Token to "UserRefreshToken" table
            var userRefreshToken = await _unitOfWork.UserRefreshToken.GetFirstOrDefaultAsync(t => t.UserID == user.Id);
            if (userRefreshToken != null)
            {
                if (DateTime.Now <= userRefreshToken.ExpiryDate)
                {
                    refreshToken = userRefreshToken.RefreshToken;
                }
                else
                {
                    _unitOfWork.UserRefreshToken.Update(new UserRefreshToken
                    {
                        UserID = user.Id,
                        RefreshToken = refreshToken,
                        ExpiryDate = DateTime.Now.AddDays(refreshTokenExpiryDays)
                    });
                    _unitOfWork.Save();
                }
            }

            return new AuthResponse
            {
                AccessToken = tokenAsString,
                IsSuccess = true,
                ExpireDate = DateTime.Now.AddDays(accessTokenExpiryDays),
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new AuthResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true,
                };

            return new AuthResponse
            {
                IsSuccess = false,
                Message = "Email did not confirm",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<AuthResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return new AuthResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to the email successfully!"
            };
        }

        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            if (model.NewPassword != model.ConfirmPassword)
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Password doesn't match its confirmation",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new AuthResponse
                {
                    Message = "Password has been reset successfully!",
                    IsSuccess = true,
                };

            return new AuthResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        public async Task<AuthResponse> RefreshToken(AuthResponse model)
        {
            var user = await GetUserFromAccessToken(model.AccessToken);
            if (user != null && (await ValidateRefreshToken(user, model.RefreshToken)))
            {

                string tokenAsString =await GenerateAccessToken(user);

                return new AuthResponse
                {
                    AccessToken = tokenAsString,
                    IsSuccess = true,
                    ExpireDate = DateTime.Now.AddDays(accessTokenExpiryDays),
                    RefreshToken = model.RefreshToken
                };
            }

            return new AuthResponse
            {
                IsSuccess = false,
                Message = "Some properties are not valid",
            };
        }

        private async Task<IdentityUser> GetUserFromAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    return await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
                }
            }
            catch (Exception)
            {
                return new IdentityUser();
            }

            return new IdentityUser();
        }

        private async Task<bool> ValidateRefreshToken(IdentityUser user, string refreshToken)
        {

            var refreshTokenUser =await _unitOfWork.UserRefreshToken
            .GetFirstOrDefaultAsync(t => t.RefreshToken == refreshToken && t.UserID == user.Id);

            if (refreshTokenUser != null && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }
    }
}
