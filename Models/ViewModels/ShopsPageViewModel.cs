using Microsoft.AspNetCore.Mvc.Rendering;

namespace GSTFiling.Models.ViewModels
{
    // Everything the Shops page needs
    public class ShopsPageViewModel
    {
        public List<ShopListItemViewModel> Shops { get; set; } = new();

        // "all", "assigned" or "unassigned"
        public string Filter { get; set; } = "all";

        // This centre's ACTIVE consultants, for the quick-assign dropdown in every row
        public List<SelectListItem> ConsultantOptions { get; set; } = new();
    }
}