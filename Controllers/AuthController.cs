using System.Security.Claims;
using GSTFiling.Data;
using GSTFiling.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // LOGIN PAGE
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var home = GetRoleHome(
                    User.FindFirstValue(ClaimTypes.Role));

                if (home != null)
                    return home;
            }

            return View();
        }

        // LOGIN SUBMIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string username,
            string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrEmpty(password))
            {
                ViewBag.EmailError =
                    "Enter your email and password.";

                return View();
            }

            var email = username.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email != null &&
                    u.Email.ToLower() == email);
            if (user == null || user.Password != password)
                return LoginFailed();

            if (user.IsActive == false)
                return AccountInactive();

            var roleName = await _context.Roles
                .Where(r => r.Id == user.RoleId)
                .Select(r => r.Name)
                .FirstOrDefaultAsync();

            if (roleName is not
                (Role.Admin or Role.Centre or
                 Role.Consultant or Role.Shop))
            {
                return LoginFailed();
            }

            var claims = CreateClaims(
                user.Id,
                user.UserName ?? email,
                email,
                roleName);

            if (user.CentreId.HasValue)
            {
                claims.Add(new Claim(
                    "CentreId",
                    user.CentreId.Value.ToString()));
            }

            if (roleName == Role.Centre &&
                user.CentreId == null)
            {
                return LoginFailed();
            }

            if (roleName == Role.Consultant)
            {
                if (user.ConsultantId == null)
                    return LoginFailed();

                claims.Add(new Claim(
                    "ConsultantId",
                    user.ConsultantId.Value.ToString()));
            }

            if (roleName == Role.Shop)
            {
                if (user.ShopId == null)
                    return LoginFailed();

                claims.Add(new Claim(
                    "ShopId",
                    user.ShopId.Value.ToString()));
            }

            return await SignInAndGoHome(claims, roleName);
        }

        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Auth");
        }

        // ACCESS DENIED
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // CREATE USER CLAIMS
        private static List<Claim> CreateClaims(
            int id,
            string name,
            string email,
            string role)
        {
            return new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    name),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    ClaimTypes.Role,
                    role)
            };
        }

        // SIGN IN AND REDIRECT
        private async Task<IActionResult> SignInAndGoHome(
            List<Claim> claims,
            string role)
        {
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return GetRoleHome(role)
                ?? RedirectToAction("Login", "Auth");
        }

        // ROLE-BASED DASHBOARD
        private IActionResult? GetRoleHome(string? role)
        {
            return role switch
            {
                Role.Admin =>
                    RedirectToAction("Dashboard", "Admin"),

                Role.Centre =>
                    RedirectToAction("Dashboard", "Firm"),

                Role.Consultant =>
                    RedirectToAction("Dashboard", "Consultant"),

                Role.Shop =>
                    RedirectToAction("Dashboard", "Shop"),

                _ => null
            };
        }

        // INVALID LOGIN
        private IActionResult LoginFailed()
        {
            ViewBag.EmailError =
                "Email or password is incorrect.";

            return View("Login");
        }

        // INACTIVE ACCOUNT
        private IActionResult AccountInactive()
        {
            ViewBag.EmailError =
                "This account is inactive. Please contact your centre.";

            return View("Login");
        }
    }
}