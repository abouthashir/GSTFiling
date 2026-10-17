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
    }
}