using System.Security.Claims;
using GSTFiling.Data;
using GSTFiling.Models;
using GSTFiling.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Controllers
{
    [Authorize(Roles = Role.Centre)]
    public class FirmController : Controller
    {
        private readonly AppDbContext _db;

        public FirmController(AppDbContext db)
        {
            _db = db;
        }

        // The logged-in Centre user's CentreId, saved in their login cookie at login.
        private int GetCurrentCentreId()
        {
            return int.Parse(User.FindFirstValue("CentreId")!);
        }

        public async Task<IActionResult> Dashboard()
        {
            int centreId = GetCurrentCentreId();

            var centre = await _db.Centres.FirstOrDefaultAsync(c => c.Id == centreId);
            if (centre == null)
            {
                return NotFound();
            }

            var model = new FirmDashboardViewModel
            {
                CentreName = centre.Name ?? "Centre",
                ShopCount = await _db.Customers.CountAsync(s => s.CentreId == centreId),
                ConsultantCount = await _db.Consultants.CountAsync(c => c.CentreId == centreId),
                UnassignedShopCount = await _db.Customers.CountAsync(s => s.CentreId == centreId && s.ConsultantId == null)
            };

            return View(model);
        }

        public async Task<IActionResult> Consultants()
        {
            int centreId = GetCurrentCentreId();

            var consultants = await _db.Consultants
                .Where(c => c.CentreId == centreId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(consultants);
        }

        [HttpGet]
        public IActionResult AddConsultant()
        {
            return View(new ConsultantFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConsultant(ConsultantFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim().ToLower();

            if (await EmailIsTakenAsync(email))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already used by another account.");
                return View(model);
            }

            int centreId = GetCurrentCentreId();

            var consultant = new Consultant
            {
                Name = model.Name.Trim(),
                Email = email,
                MobileNumber = model.MobileNumber,
                Address = model.Address?.Trim(),
                IsActive = true,
                CentreId = centreId,
                UpdatedAt = DateTime.Now
            };

            try
            {
                await CreateConsultantWithLoginAsync(consultant, model.Password, centreId);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already used by another account.");
                return View(model);
            }

            TempData["Success"] = $"{consultant.Name} was added. They can log in with {email}.";
            return RedirectToAction(nameof(Consultants));
        }

        [HttpGet]
        public async Task<IActionResult> EditConsultant(int id)
        {
            int centreId = GetCurrentCentreId();

            var consultant = await _db.Consultants
                .FirstOrDefaultAsync(c => c.Id == id && c.CentreId == centreId);

            if (consultant == null)
            {
                return NotFound();
            }

            var model = new EditConsultantViewModel
            {
                Id = consultant.Id,
                Name = consultant.Name ?? string.Empty,
                Email = consultant.Email ?? string.Empty,
                MobileNumber = consultant.MobileNumber ?? string.Empty,
                Address = consultant.Address,
                IsActive = consultant.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConsultant(EditConsultantViewModel model)
        {
            int centreId = GetCurrentCentreId();

            var consultant = await _db.Consultants
                .FirstOrDefaultAsync(c => c.Id == model.Id && c.CentreId == centreId);

            if (consultant == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim().ToLower();

            var login = await _db.Users
                .FirstOrDefaultAsync(u => u.ConsultantId == consultant.Id);

            if (await EmailIsTakenAsync(email, login?.Id))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already used by another account.");
                return View(model);
            }

            // 1. The details row
            consultant.Name = model.Name.Trim();
            consultant.Email = email;
            consultant.MobileNumber = model.MobileNumber;
            consultant.Address = model.Address?.Trim();
            consultant.IsActive = model.IsActive;
            consultant.UpdatedAt = DateTime.Now;

            // 2. The login row
            if (login != null)
            {
                login.UserName = consultant.Name;
                login.Email = email;
                login.IsActive = model.IsActive;
                login.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    login.Password = model.NewPassword;
                }
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already used by another account.");
                return View(model);
            }

            TempData["Success"] = $"{consultant.Name} was updated.";
            return RedirectToAction(nameof(Consultants));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConsultant(int id)
        {
            int centreId = GetCurrentCentreId();

            var consultant = await _db.Consultants
                .FirstOrDefaultAsync(c => c.Id == id && c.CentreId == centreId);

            if (consultant == null)
            {
                return NotFound();
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            // 1. Keep their shops, but take the consultant off them
            int unassignedShops = await _db.Customers
                .Where(s => s.ConsultantId == consultant.Id && s.CentreId == centreId)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.ConsultantId, (int?)null));

            // 2. Delete their login
            await _db.Users
                .Where(u => u.ConsultantId == consultant.Id)
                .ExecuteDeleteAsync();

            // 3. Delete the consultant
            _db.Consultants.Remove(consultant);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["Success"] = unassignedShops > 0
                ? $"{consultant.Name} was deleted. {unassignedShops} shop(s) now need a new consultant."
                : $"{consultant.Name} was deleted.";

            return RedirectToAction(nameof(Consultants));
        }

        // ---------- Helpers ----------

        private async Task<bool> EmailIsTakenAsync(string email, int? exceptUserId = null)
        {
            return await _db.Users.AnyAsync(u =>
                u.Email != null &&
                u.Email.ToLower() == email &&
                u.Id != exceptUserId);
        }

        private async Task CreateConsultantWithLoginAsync(Consultant consultant, string password, int centreId)
        {
            int consultantRoleId = await _db.Roles
                .Where(r => r.Name == Role.Consultant)
                .Select(r => r.Id)
                .FirstAsync();

            await using var transaction = await _db.Database.BeginTransactionAsync();

            _db.Consultants.Add(consultant);
            await _db.SaveChangesAsync();

            _db.Users.Add(new User
            {
                UserName = consultant.Name,
                Email = consultant.Email,
                Password = password,
                RoleId = consultantRoleId,
                IsActive = true,
                CentreId = centreId,
                ConsultantId = consultant.Id,
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
    }
}