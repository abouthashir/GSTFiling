using GSTFiling.Data;
using GSTFiling.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Controllers
{
    public class FirmController : Controller
    {
        private readonly AppDbContext _db;

        public FirmController(AppDbContext db)
        {
            _db = db;
        }

        // Temporary: until login exists, we act as Centre 1 (Kochi Centre).
        // When login is added, ONLY this method changes.
        private int GetCurrentCentreId()
        {
            return 1;
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