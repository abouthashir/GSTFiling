namespace GSTFiling.Models.ViewModels
{
    public class FirmDashboardViewModel
    {
        public string CentreName { get; set; } = string.Empty;

        public int ShopCount { get; set; }

        public int ConsultantCount { get; set; }

        public int UnassignedShopCount { get; set; }
    }
}