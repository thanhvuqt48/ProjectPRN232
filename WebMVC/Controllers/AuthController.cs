using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObjects.Consts;
using BusinessObjects.Dtos;
using BusinessObjects.Dtos.Auth;
using BusinessObjects.Dtos.response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebMVC.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly HttpClient _httpClient;

        public AuthController(ILogger<AuthController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5290");
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    // Copy cookies from API response to MVC response
                    if (response.Headers.Contains("Set-Cookie"))
                    {
                        var cookies = response.Headers.GetValues("Set-Cookie");
                        foreach (var cookie in cookies)
                        {
                            Response.Headers.Add("Set-Cookie", cookie);
                        }
                    }

                    TempData["SuccessMessage"] = "Đăng nhập thành công! Đang chuyển hướng đến trang chủ...";
                    return RedirectToAction("Index", "Home");
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Đăng nhập thất bại: " + error);

                TempData["ErrorMessage"] = "Email hoặc mật khẩu không đúng. Vui lòng thử lại.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng nhập hệ thống");
                TempData["ErrorMessage"] = "Hệ thống đang gặp sự cố. Vui lòng thử lại sau.";
                return RedirectToAction("Login", "Auth");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        [HttpGet("ExternalLogin")]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { provider }, Request.Scheme);
            return Redirect($"http://localhost:5290/api/auth/external-login?provider={provider}&redirectUri={Uri.EscapeDataString(redirectUrl)}");
        }

        [HttpGet("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string provider)
        {
            return Redirect($"http://localhost:5290/api/auth/external-login-callback?provider={provider}");
        }

        // ✅ THÊM ACTION MỚI để xử lý thành công external login
        [HttpGet("ExternalLoginSuccess")]
        public async Task<IActionResult> ExternalLoginSuccess(int accountId, string firstName, string lastName, string email, string provider)
        {
            try
            {
                // ✅ TẠO CLAIMS CHO MVC APPLICATION
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, accountId.ToString()),
                    new Claim(ClaimTypes.Name, $"{firstName} {lastName}".Trim()),
                    new Claim(ClaimTypes.GivenName, firstName ?? ""),
                    new Claim(ClaimTypes.Surname, lastName ?? ""),
                    new Claim(ClaimTypes.Email, email),
                    new Claim("Provider", provider)
                };

                var identity = new ClaimsIdentity(claims, AuthSchemes.Cookie);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(AuthSchemes.Cookie, principal, authProperties);

                // ✅ Set additional cookies trong MVC
                Response.Cookies.Append("AccountId", accountId.ToString(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                Response.Cookies.Append("AccountName", $"{firstName} {lastName}".Trim(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                Response.Cookies.Append("Email", email, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                Response.Cookies.Append("LoginProvider", provider, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // ✅ Set session thông tin (backup)
                if (HttpContext.Session != null)
                {
                    HttpContext.Session.SetInt32("AccountId", accountId);
                    HttpContext.Session.SetString("AccountName", $"{firstName} {lastName}".Trim());
                    HttpContext.Session.SetString("Email", email);
                    HttpContext.Session.SetString("LoginProvider", provider);
                }

                TempData["SuccessMessage"] = "Đăng nhập thành công! Đang chuyển hướng đến trang chủ...";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xử lý đăng nhập external");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.";
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("CompleteRegistration")]
        public IActionResult CompleteRegistration()
        {
            var dto = new ExternalAccountRegisterDto
            {
                Email = Request.Query["email"].ToString(),
                AuthProvider = Request.Query["provider"].ToString(),
                AuthProviderId = Request.Query["providerId"].ToString(), // Đảm bảo lấy đúng providerId
                FirstName = Request.Query["firstName"].ToString(),
                LastName = Request.Query["lastName"].ToString()
            };

            // Kiểm tra thông tin có đầy đủ không
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.AuthProvider) || string.IsNullOrEmpty(dto.AuthProviderId))
            {
                TempData["ErrorMessage"] = "Thiếu thông tin cần thiết để hoàn tất đăng ký. Vui lòng thử lại.";
                return RedirectToAction("Login", "Auth");
            }

            return View(dto);
        }

        [HttpPost("CompleteRegistration")]
        public async Task<IActionResult> CompleteRegistration(ExternalAccountRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // Kiểm tra lại AuthProviderId trước khi gửi request
            if (string.IsNullOrEmpty(dto.AuthProviderId))
            {
                TempData["ErrorMessage"] = "Thiếu thông tin xác thực. Vui lòng thử đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/auth/complete-registration", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var registrationResponse = JsonSerializer.Deserialize<RegistrationResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Lưu thông tin vào session
                    if (HttpContext.Session != null)
                    {
                        HttpContext.Session.SetInt32("AccountId", registrationResponse.AccountId);
                        HttpContext.Session.SetString("AccountName", $"{dto.FirstName} {dto.LastName}".Trim());
                        HttpContext.Session.SetString("Email", registrationResponse.Email);
                        HttpContext.Session.SetString("LoginProvider", dto.AuthProvider ?? "");
                    }

                    TempData["SuccessRegisMessage"] = "Đăng ký tài khoản thành công! Đang chuyển hướng bạn đến trang chủ...";
                    TempData["RedirectUrl"] = registrationResponse.RedirectUrl;
                    return RedirectToAction("Index", "Home");
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Hoàn tất đăng ký thất bại: " + error);

                // Xử lý lỗi cụ thể
                if (error.Contains("AuthProviderId"))
                {
                    TempData["ErrorMessage"] = "Thông tin xác thực không hợp lệ. Vui lòng thử đăng nhập lại.";
                    return RedirectToAction("Login", "Auth");
                }

                TempData["ErrorMessage"] = "Không thể hoàn tất đăng ký. Vui lòng thử lại.";
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hoàn tất đăng ký");
                TempData["ErrorMessage"] = "Hệ thống đang gặp sự cố. Vui lòng thử lại sau.";
                return View(dto);
            }
        }
    }
}