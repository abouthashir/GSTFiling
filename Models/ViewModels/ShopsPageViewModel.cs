namespace GSTFiling.Models.ViewModels
{
    // Everything the Shops page needs
    public class ShopsPageViewModel
    {
        public List<ShopListItemViewModel> Shops { get; set; } = new();

        // "all", "assigned" or "unassigned"
        public string Filter { get; set; } = "all";
    }
}