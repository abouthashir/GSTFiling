using GSTFiling.Data;
using GSTFiling.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GSTFiling.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        private const int CentreRoleId = 2;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalCentres =
                await _context.Centres.CountAsync();

            ViewBag.ActiveCentres =
                await _context.Centres.CountAsync(c => c.IsActive);

            ViewBag.InactiveCentres =
                await _context.Centres.CountAsync(c => !c.IsActive);

            ViewBag.TotalClients =
                await _context.Customers.CountAsync();

            return View();
        }

        // 2. Centres List
        public async Task<IActionResult> Centres()
        {
            var centres = await _context.Centres
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(centres);
        }

        // 3. Toggle Centre Active / Inactive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var centre = await _context.Centres
                .FirstOrDefaultAsync(c => c.Id == id);

            if (centre == null)
                return NotFound();

            centre.IsActive = !centre.IsActive;
            centre.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Centre status updated successfully.";

            return RedirectToAction(nameof(Centres));
        }

        // 4. Register Centre - GET
        [HttpGet]
        public IActionResult RegisterCentre()
        {
            return View();
        }

        // 5. Register Centre - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCentre(
            Centre centre, string? password)
        {
            if (string.IsNullOrWhiteSpace(centre.Name))
            {
                ModelState.AddModelError(
                    "Name", "Centre name is required.");
            }

            if (string.IsNullOrWhiteSpace(centre.Email))
            {
                ModelState.AddModelError(
                    "Email", "Centre email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(
                    "password", "Password is required.");
            }

            if (!ModelState.IsValid)
                return View(centre);

            var centreName = centre.Name!.Trim();
            var email = centre.Email!.Trim();

            bool emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email != null &&
                    u.Email.ToLower() == email.ToLower());

            if (emailExists)
            {
                ModelState.AddModelError(
                    "Email",
                    "This email is already registered.");

                return View(centre);
            }

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                centre.Name = centreName;
                centre.Email = email;
                centre.IsActive = true;
                centre.UpdatedAt = DateTime.Now;

                _context.Centres.Add(centre);
                await _context.SaveChangesAsync();

                var user = new User
                {
                    UserName = centreName,
                    Email = email,
                    RoleId = CentreRoleId,
                    CentreId = centre.Id,
                    IsActive = true,
                    UpdatedAt = DateTime.Now.ToString(
                        CultureInfo.InvariantCulture)
                };

                user.Password = password;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                centre.UserId = user.Id;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] =
                    "Centre and login account registered successfully.";

                return RedirectToAction(nameof(Centres));
            }
            catch
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    "",
                    "Registration failed. Please check the details.");

                return View(centre);
            }
        }

        // 6. Master Data Page
        public async Task<IActionResult> MasterData()
        {
            ViewBag.FinancialYears =
                await _context.FinancialYears
                    .OrderBy(x => x.Id)
                    .ToListAsync();

            ViewBag.FilingMonths =
                await _context.FilingMonths
                    .OrderBy(x => x.Id)
                    .ToListAsync();

            ViewBag.GstTaxRates =
                await _context.GstTaxRates
                    .OrderBy(x => x.Id)
                    .ToListAsync();

            ViewBag.InvoiceClassifications =
                await _context.InvoiceClassifications
                    .OrderBy(x => x.Id)
                    .ToListAsync();

            return View();
        }

        // 7. Add Financial Year
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFinancialYear(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return RedirectToAction(nameof(MasterData));

            var year = new FinancialYear
            {
                Name = name.Trim(),
                IsActive = true
            };

            _context.FinancialYears.Add(year);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MasterData));
        }

        // 8. Add Filing Month
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFilingMonth(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return RedirectToAction(nameof(MasterData));

            var month = new FilingMonths
            {
                Name = name.Trim(),
                IsActive = true
            };

            _context.FilingMonths.Add(month);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MasterData));
        }

        // 9. Add GST Tax Rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGstTaxRate(decimal rate)
        {
            if (rate < 0 || rate > 999.99m)
            {
                TempData["Error"] =
                    "Enter a valid GST tax rate.";

                return RedirectToAction(nameof(MasterData));
            }

            var taxRate = new GstTaxRates
            {
                Rate = rate,
                IsActive = true
            };

            _context.GstTaxRates.Add(taxRate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MasterData));
        }

        // 10. Add Invoice Classification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInvoiceClassification(
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return RedirectToAction(nameof(MasterData));

            var classification = new InvoiceClassification
            {
                Name = name.Trim(),
                IsActive = true
            };

            _context.InvoiceClassifications.Add(classification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MasterData));
        }

        // 11. Delete Financial Year
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFinancialYear(int id)
        {
            var item = await _context.FinancialYears.FindAsync(id);

            if (item != null)
            {
                _context.FinancialYears.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MasterData));
        }

        // 12. Delete Filing Month
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFilingMonth(int id)
        {
            var item = await _context.FilingMonths.FindAsync(id);

            if (item != null)
            {
                _context.FilingMonths.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MasterData));
        }

        // 13. Delete GST Tax Rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGstTaxRate(int id)
        {
            var item = await _context.GstTaxRates.FindAsync(id);

            if (item != null)
            {
                _context.GstTaxRates.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MasterData));
        }

        // 14. Delete Invoice Classification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoiceClassification(
            int id)
        {
            var item =
                await _context.InvoiceClassifications.FindAsync(id);

            if (item != null)
            {
                _context.InvoiceClassifications.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MasterData));
        }

        // 15. View Centre Details, Shops and Consultants
        [HttpGet]
        public async Task<IActionResult> ViewCentre(int id)
        {
            var centre = await _context.Centres
                .FirstOrDefaultAsync(c => c.Id == id);

            if (centre == null)
                return NotFound();

            // Shops/customers registered under this centre
            var shops = await _context.Customers
                .Where(c => c.CentreId == id)
                .OrderBy(c => c.ShopName)
                .ToListAsync();

            // Consultants assigned to this centre
            var consultants = await _context.Consultants
                .Where(c => c.CentreId == id)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Shops = shops;
            ViewBag.Consultants = consultants;
            ViewBag.TotalShops = shops.Count;
            ViewBag.TotalConsultants = consultants.Count;
            ViewBag.ActiveConsultants =
                consultants.Count(c => c.IsActive);
            ViewBag.InactiveConsultants =
                consultants.Count(c => !c.IsActive);

            return View(centre);
        }

        // 16. Edit Centre - GET
        [HttpGet]
        public async Task<IActionResult> EditCentre(int id)
        {
            var centre = await _context.Centres
                .FirstOrDefaultAsync(c => c.Id == id);

            if (centre == null)
                return NotFound();

            return View(centre);
        }

        // 17. Edit Centre - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCentre(
            Centre centre, string? NewPassword)
        {
            if (centre.Id <= 0)
                return BadRequest();

            var existing = await _context.Centres
                .FirstOrDefaultAsync(c => c.Id == centre.Id);

            if (existing == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(centre.Name))
            {
                TempData["Error"] =
                    "Centre name is required.";

                return RedirectToAction(nameof(Centres));
            }

            User? linkedUser = null;

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                if (existing.UserId == null)
                {
                    TempData["Error"] =
                        "This centre has no linked user account. Password was not changed.";

                    return RedirectToAction(nameof(Centres));
                }

                linkedUser = await _context.Users
                    .FirstOrDefaultAsync(
                        u => u.Id == existing.UserId.Value);

                if (linkedUser == null)
                {
                    TempData["Error"] =
                        "The linked user account was not found. Password was not changed.";

                    return RedirectToAction(nameof(Centres));
                }
            }

            existing.Name = centre.Name.Trim();
            existing.Address = centre.Address;
            existing.Email = centre.Email;
            existing.ContactName = centre.ContactName;
            existing.MobileNumber = centre.MobileNumber;
            existing.UpdatedAt = DateTime.Now;

            if (linkedUser != null &&
                !string.IsNullOrWhiteSpace(NewPassword))
            {
                linkedUser.Password = NewPassword;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Centre details updated successfully.";

            return RedirectToAction(nameof(Centres));
        }

        // 18. Delete Centre
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCentre(int id)
        {
            var centre = await _context.Centres
                .FirstOrDefaultAsync(c => c.Id == id);

            if (centre != null)
            {
                _context.Centres.Remove(centre);
                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Centre deleted successfully.";
            }

            return RedirectToAction(nameof(Centres));
        }
    }
}