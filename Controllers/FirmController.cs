using GSTFiling.Data;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}