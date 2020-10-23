using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Tuteexy.Api.Service;
using Tuteexy.Models;

namespace Tuteexy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private IAuthService _authService;
        private IConfiguration _configuration;
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        // /api/auth/register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterUserAsync(model);

                if (result.IsSuccess)
                    return Ok(result); // Status Code: 200 

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid"); // Status code: 400
        }

        // /api/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginUserAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        // /api/auth/confirmemail?userid&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            }

            return BadRequest(result);
        }

        // api/auth/forgetpassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _authService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }

        // api/auth/resetpassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }


        // api/auth/RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponse refreshRequest)
        {

            if (ModelState.IsValid)
            {
                var result = await _authService.RefreshToken(refreshRequest);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

    }
}
