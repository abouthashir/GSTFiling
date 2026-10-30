using System.Security.Claims;
using GSTFiling.Data;
using GSTFiling.Models;
using GSTFiling.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<IActionResult> Shops(string? filter)
        {
            int centreId = GetCurrentCentreId();

            // Only accept the 3 known filter values
            filter = filter?.ToLower() switch
            {
                "assigned" => "assigned",
                "unassigned" => "unassigned",
                _ => "all"
            };

            // LEFT JOIN: every shop of this centre + its consultant's name (if any)
            var query =
                from s in _db.Customers
                where s.CentreId == centreId
                join c in _db.Consultants on s.ConsultantId equals (int?)c.Id into consultantGroup
                from c in consultantGroup.DefaultIfEmpty()
                select new ShopListItemViewModel
                {
                    Id = s.Id,
                    ShopName = s.ShopName ?? string.Empty,
                    OwnerName = s.Name,
                    GstNumber = s.GstNumber,
                    MobileNumber = s.MobileNumber,
                    Email = s.Email,
                    ConsultantId = s.ConsultantId,
                    ConsultantName = c != null ? c.Name : null
                };

            if (filter == "assigned")
            {
                query = query.Where(x => x.ConsultantId != null);
            }
            else if (filter == "unassigned")
            {
                query = query.Where(x => x.ConsultantId == null);
            }

            var model = new ShopsPageViewModel
            {
                Shops = await query.OrderBy(x => x.ShopName).ToListAsync(),
                Filter = filter,
                ConsultantOptions = await GetConsultantOptionsAsync(centreId)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddShop()
        {
            var model = new ShopFormViewModel
            {
                ConsultantOptions = await GetConsultantOptionsAsync(GetCurrentCentreId())
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddShop(ShopFormViewModel model)
        {
            int centreId = GetCurrentCentreId();

            // The chosen consultant must be one of MY active consultants
            if (model.ConsultantId != null &&
                !await IsMyActiveConsultantAsync(model.ConsultantId.Value, centreId))
            {
                ModelState.AddModelError(nameof(model.ConsultantId), "Choose a consultant from the list.");
            }

            if (!ModelState.IsValid)
            {
                model.ConsultantOptions = await GetConsultantOptionsAsync(centreId);
                return View(model);
            }

            var email = model.Email.Trim().ToLower();

            if (await EmailIsTakenAsync(email))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already used by another account.");
                model.ConsultantOptions = await GetConsultantOptionsAsync(centreId);
                return View(model);
            }

            var shop = new Customer
            {
                ShopName = model.ShopName.Trim(),
                Name = model.OwnerName.Trim(),
                GstNumber = model.GstNumber.Trim().ToUpper(),
                Email = email,
                MobileNumber = model.MobileNumber,
                Address = model.Address?.Trim(),
                CentreId = centreId,
                ConsultantId = model.ConsultantId,
                UpdatedAt = DateTime.Now
            };

            try
            {
                await CreateShopWithLoginAsync(shop, model.Password, centreId);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already used by another account.");
                model.ConsultantOptions = await GetConsultantOptionsAsync(centreId);
                return View(model);
            }

            TempData["Success"] = $"{shop.ShopName} was added. They can log in with {email}.";
            return RedirectToAction(nameof(Shops));
        }

        [HttpGet]
        public async Task<IActionResult> EditShop(int id)
        {
            int centreId = GetCurrentCentreId();

            var shop = await _db.Customers
                .FirstOrDefaultAsync(s => s.Id == id && s.CentreId == centreId);

            if (shop == null)
            {
                return NotFound();
            }

            var model = new EditShopViewModel
            {
                Id = shop.Id,
                ShopName = shop.ShopName ?? string.Empty,
                OwnerName = shop.Name ?? string.Empty,
                GstNumber = shop.GstNumber ?? string.Empty,
                MobileNumber = shop.MobileNumber ?? string.Empty,
                Email = shop.Email ?? string.Empty,
                Address = shop.Address,
                ConsultantId = shop.ConsultantId,
                ConsultantOptions = await GetConsultantOptionsAsync(centreId, shop.ConsultantId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShop(EditShopViewModel model)
        {
            int centreId = GetCurrentCentreId();

            var shop = await _db.Customers
                .FirstOrDefaultAsync(s => s.Id == model.Id && s.CentreId == centreId);

            if (shop == null)
            {
                return NotFound();
            }

            // Keeping the same consultant is always fine.
            // Changing to another one: it must be MY active consultant.
            if (model.ConsultantId != null &&
                model.ConsultantId != shop.ConsultantId &&
                !await IsMyActiveConsultantAsync(model.ConsultantId.Value, centreId))
            {
                ModelState.AddModelError(nameof(model.ConsultantId), "Choose a consultant from the list.");
            }

            if (!ModelState.IsValid)
            {
                model.ConsultantOptions = await GetConsultantOptionsAsync(centreId, shop.ConsultantId);
                return View(model);
            }

            var email = model.Email.Trim().ToLower();

            var login = await _db.Users
                .FirstOrDefaultAsync(u => u.ShopId == shop.Id);

            if (await EmailIsTakenAsync(email, login?.Id))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already used by another account.");
                model.ConsultantOptions = await GetConsultantOptionsAsync(centreId, shop.ConsultantId);
                return View(model);
            }

            // 1. The shop row
            shop.ShopName = model.ShopName.Trim();
            shop.Name = model.OwnerName.Trim();
            shop.GstNumber = model.GstNumber.Trim().ToUpper();
            shop.MobileNumber = model.MobileNumber;
            shop.Email = email;
            shop.Address = model.Address?.Trim();
            shop.ConsultantId = model.ConsultantId;
            shop.UpdatedAt = DateTime.Now;

            // 2. The login row
            if (login != null)
            {
                login.UserName = shop.ShopName;
                login.Email = email;
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
                model.ConsultantOptions = await GetConsultantOptionsAsync(centreId, shop.ConsultantId);
                return View(model);
            }

            TempData["Success"] = $"{shop.ShopName} was updated.";
            return RedirectToAction(nameof(Shops));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignConsultant(int shopId, int? consultantId, string? returnFilter)
        {
            int centreId = GetCurrentCentreId();

            var shop = await _db.Customers
                .FirstOrDefaultAsync(s => s.Id == shopId && s.CentreId == centreId);

            if (shop == null)
            {
                return NotFound();
            }

            // Nothing changed: just go back
            if (consultantId == shop.ConsultantId)
            {
                return RedirectToAction(nameof(Shops), new { filter = returnFilter });
            }

            // A new consultant must be one of MY active consultants
            if (consultantId != null && !await IsMyActiveConsultantAsync(consultantId.Value, centreId))
            {
                TempData["Error"] = "Choose an active consultant from your centre.";
                return RedirectToAction(nameof(Shops), new { filter = returnFilter });
            }

            shop.ConsultantId = consultantId;
            shop.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            if (consultantId == null)
            {
                TempData["Success"] = $"{shop.ShopName} is now not assigned.";
            }
            else
            {
                var consultantName = await _db.Consultants
                    .Where(c => c.Id == consultantId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();

                TempData["Success"] = $"{shop.ShopName} is now assigned to {consultantName}.";
            }

            return RedirectToAction(nameof(Shops), new { filter = returnFilter });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShop(int id)
        {
            int centreId = GetCurrentCentreId();

            var shop = await _db.Customers
                .FirstOrDefaultAsync(s => s.Id == id && s.CentreId == centreId);

            if (shop == null)
            {
                return NotFound();
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            // 1. Delete the shop's login
            await _db.Users
                .Where(u => u.ShopId == shop.Id)
                .ExecuteDeleteAsync();

            // 2. Delete the shop
            _db.Customers.Remove(shop);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["Success"] = $"{shop.ShopName} and its login were deleted.";
            return RedirectToAction(nameof(Shops));
        }

        // View-only list of one shop's invoices. The Centre cannot add, edit or delete them.
        public async Task<IActionResult> ShopInvoices(int id, int? year, string? month)
        {
            int centreId = GetCurrentCentreId();

            // The shop must belong to MY centre
            var shop = await _db.Customers
                .FirstOrDefaultAsync(s => s.Id == id && s.CentreId == centreId);

            if (shop == null)
            {
                return NotFound();
            }

            // Only accept a real month name from the URL
            if (month != null && !ShopInvoicesPageViewModel.MonthNames.Contains(month))
            {
                month = null;
            }

            // This shop's invoices; the filters are added below
            var query = _db.Invoice.Where(i => i.cstmrid == shop.Id);

            if (year != null)
            {
                query = query.Where(i => i.invyear == year);
            }

            if (month != null)
            {
                query = query.Where(i => i.invmonth == month);
            }

            var model = new ShopInvoicesPageViewModel
            {
                ShopId = shop.Id,
                ShopName = shop.ShopName ?? "Shop",
                OwnerName = shop.Name,
                GstNumber = shop.GstNumber,
                Year = year,
                Month = month,

                YearOptions = await _db.Invoice
                    .Where(i => i.cstmrid == shop.Id)
                    .Select(i => i.invyear)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToListAsync(),

                Invoices = await query
                    .OrderByDescending(i => i.invid)
                    .Select(i => new ShopInvoiceRowViewModel
                    {
                        Id = i.invid,
                        InvoiceNumber = i.invno,
                        InvoiceDate = i.invdate,
                        TaxRate = i.taxrate,
                        TaxableAmount = i.taxable_amt,
                        Cgst = i.cgst,
                        Sgst = i.sgst,
                        Cess = i.cess,
                        GstAmount = i.gst_amt
                    })
                    .ToListAsync()
            };

            return View(model);
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

        // Dropdown choices: this centre's ACTIVE consultants, A-Z.
        // keepConsultantId: also show this one even if inactive (the shop's current consultant).
        private async Task<List<SelectListItem>> GetConsultantOptionsAsync(int centreId, int? keepConsultantId = null)
        {
            return await _db.Consultants
                .Where(c => c.CentreId == centreId && (c.IsActive || c.Id == keepConsultantId))
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.IsActive ? c.Name : c.Name + " (inactive)"
                })
                .ToListAsync();
        }

        private async Task<bool> IsMyActiveConsultantAsync(int consultantId, int centreId)
        {
            return await _db.Consultants.AnyAsync(c =>
                c.Id == consultantId &&
                c.CentreId == centreId &&
                c.IsActive);
        }

        private async Task CreateShopWithLoginAsync(Customer shop, string password, int centreId)
        {
            int shopRoleId = await _db.Roles
                .Where(r => r.Name == Role.Shop)
                .Select(r => r.Id)
                .FirstAsync();

            await using var transaction = await _db.Database.BeginTransactionAsync();

            _db.Customers.Add(shop);
            await _db.SaveChangesAsync();

            _db.Users.Add(new User
            {
                UserName = shop.ShopName,
                Email = shop.Email,
                Password = password,
                RoleId = shopRoleId,
                IsActive = true,
                CentreId = centreId,
                ShopId = shop.Id,
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
    }
}