using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessObjects.Consts;
using BusinessObjects.Domains;
using BusinessObjects.Dtos;
using BusinessObjects.Dtos.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;
        private readonly IMailService _mailService;
        public AuthController(IAuthService authService, IAccountService systemAccountService, IConfiguration config, IMailService mailService)
        {
            _authService = authService;
            _accountService = systemAccountService;
            _config = config;
            _mailService = mailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Account>> Register(RegisterDto model)
        {
            var account = await _authService.RegisterAsync(model);
            if (account != null)
            {
                return Ok(account);
            }
            return BadRequest("Email already exists.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            if (result == null)
            {
                return BadRequest("Invalid username or password");
            }
            _authService.setTokensInsideCookie(result, HttpContext);
            Response.Cookies.Append("accessToken", result.AccessToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(1)
            });
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token. ");
            }
            return Ok(result);
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string redirectUri)
        {
            var scheme = provider.ToLower() switch
            {
                AuthProviders.Google => AuthSchemes.Google,
                AuthProviders.Facebook => AuthSchemes.Facebook,
                _ => throw new ArgumentException("Provider không hợp lệ", nameof(provider))
            };

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUri
            };

            return Challenge(properties, scheme);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string provider)
        {
            var scheme = provider.ToLower() switch
            {
                AuthProviders.Google => AuthSchemes.Google,
                AuthProviders.Facebook => AuthSchemes.Facebook,
                _ => throw new ArgumentException("Provider không hợp lệ", nameof(provider))
            };

            var result = await HttpContext.AuthenticateAsync(scheme);
            if (!result.Succeeded)
            {
                return BadRequest("Xác thực không thành công.");
            }

            var principal = result.Principal;
            var externalId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;
            var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value;

            if (string.IsNullOrEmpty(externalId) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Tài khoản không hợp lệ.");
            }

            var account = await _accountService.GetAccountByEmailAsync(email);
            if (account == null)
            {
                // Chuyển hướng đến trang đăng ký
                var redirectUrl = $"https://localhost:7031/Auth/CompleteRegistration?" +
                                  $"email={Uri.EscapeDataString(email)}" +
                                  $"&firstName={Uri.EscapeDataString(firstName ?? name)}" +
                                  $"&lastName={Uri.EscapeDataString(lastName ?? "")}" +
                                  $"&provider={provider}" +
                                  $"&providerId={Uri.EscapeDataString(externalId)}";
                return Redirect(redirectUrl);
            }

            // ✅ TẠO TOKEN CHO USER ĐÃ TỒN TẠI
            var tokenResponse = await _authService.GenerateTokenForExternalLoginAsync(account);
            if (tokenResponse == null)
            {
                return BadRequest("Không thể tạo token xác thực.");
            }

            // ✅ Set tokens vào cookies
            _authService.setTokensInsideCookie(tokenResponse, HttpContext);

            // ✅ TẠO AUTHENTICATION COOKIE CHO MVC APPLICATION (QUAN TRỌNG!)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Name, $"{firstName ?? ""} {lastName ?? ""}".Trim()),
                new Claim(ClaimTypes.GivenName, firstName ?? ""),
                new Claim(ClaimTypes.Surname, lastName ?? ""),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role),
                new Claim("Provider", provider) // Thêm provider claim
            };

            var identity = new ClaimsIdentity(claims, AuthSchemes.Cookie); // Sử dụng Cookie scheme
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Set authentication cookie với thời gian hết hạn
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7), // 7 ngày
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(AuthSchemes.Cookie, claimsPrincipal, authProperties);

            // ✅ REDIRECT TRỰC TIẾP VỀ MVC CONTROLLER để xử lý cookies
            var callbackUrl = $"https://localhost:7031/Auth/ExternalLoginSuccess?" +
                             $"accountId={account.AccountId}" +
                             $"&firstName={Uri.EscapeDataString(firstName ?? "")}" +
                             $"&lastName={Uri.EscapeDataString(lastName ?? "")}" +
                             $"&email={Uri.EscapeDataString(account.Email)}" +
                             $"&provider={Uri.EscapeDataString(provider)}";

            return Redirect(callbackUrl);
        }

        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] ExternalAccountRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra các thông tin cần thiết
            if (string.IsNullOrEmpty(dto.AuthProviderId))
            {
                return BadRequest(new { ErrorMessage = "AuthProviderId không được để trống." });
            }

            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { ErrorMessage = "Email không được để trống." });
            }

            if (string.IsNullOrEmpty(dto.FirstName))
            {
                return BadRequest(new { ErrorMessage = "FirstName không được để trống." });
            }

            try
            {
                // Không dựa vào HttpContext.AuthenticateAsync nữa
                // Thay vào đó, sử dụng thông tin đã được truyền từ client
                var account = await _accountService.CreateExternalAccountAsync(dto);

                if (account == null)
                {
                    return BadRequest(new { ErrorMessage = "Không thể tạo tài khoản. Vui lòng thử lại." });
                }

                // Tạo claims cho user mới
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                    new Claim(ClaimTypes.Name, $"{dto.FirstName} {dto.LastName}".Trim()),
                    new Claim(ClaimTypes.GivenName, dto.FirstName ?? ""),
                    new Claim(ClaimTypes.Surname, dto.LastName ?? ""),
                    new Claim(ClaimTypes.Email, dto.Email),
                    new Claim(ClaimTypes.Role, account.Role)
                };

                var scheme = dto.AuthProvider.ToLower() switch
                {
                    "google" => AuthSchemes.Google,
                    "facebook" => AuthSchemes.Facebook,
                    _ => AuthSchemes.Cookie // fallback to cookie scheme
                };

                var identity = new ClaimsIdentity(claims, scheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(AuthSchemes.Cookie, principal);

                // Gửi email welcome (với try-catch để tránh lỗi email làm crash app)
                try
                {
                    var baseUrl = _config["AppSettings:BaseUrl"];
                    var mail = new MailContent
                    {
                        To = dto.Email,
                        Subject = "Chào mừng đến với BlueHouse!",
                        Body = $@"
                            <div style='font-family:Arial,sans-serif;padding:20px;color:#333'>
                                <h2>Chào mừng {dto.FirstName} {dto.LastName} đến với <span style='color:#007bff;'>BlueHouse</span>!</h2>
                                <p>Cảm ơn bạn đã đăng ký tài khoản bằng {dto.AuthProvider}. Chúng tôi rất vui khi có bạn là một phần của cộng đồng.</p>
                                <img src='https://localhost:7046/images/welcome-mail.jpg' alt='Welcome' style='max-width:100%;border-radius:10px;margin:20px 0' />
                                <p>Bắt đầu hành trình của bạn ngay hôm nay bằng cách khám phá các tính năng tuyệt vời của BlueHouse.</p>
                                <a href='{baseUrl}' style='display:inline-block;padding:10px 20px;background:#4b69bd;color:#fff;text-decoration:none;border-radius:5px;'>Truy cập BlueHouse</a>
                            </div>"
                    };

                    await _mailService.SendMail(mail);
                }
                catch (Exception emailEx)
                {
                    // Log lỗi email nhưng không làm crash registration process
                    // Có thể log: _logger.LogError(emailEx, "Failed to send welcome email to {Email}", dto.Email);
                    // Không return lỗi vì account đã được tạo thành công
                }

                return Ok(new
                {
                    AccountId = account.AccountId,
                    Email = account.Email,
                    Role = account.Role,
                    RedirectUrl = Url.Action("Index", "Home", null, Request.Scheme)
                });
            }
            catch (Exception ex)
            {
                // Log lỗi và return generic error
                // _logger.LogError(ex, "Error completing registration for {Email}", dto.Email);
                return BadRequest(new { ErrorMessage = "Có lỗi xảy ra khi tạo tài khoản. Vui lòng thử lại." });
            }
        }
    }
}