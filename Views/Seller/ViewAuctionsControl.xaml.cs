using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BidUp_App.Models.Users;

namespace BidUp_App.Views.Seller
{
    public partial class ViewAuctionsControl : UserControl
    {
        private readonly int _sellerId;
        private readonly DataContextDataContext _dbContext;
        private readonly SellerDashboard _parentDashboard;

        public ViewAuctionsControl(int sellerId, SellerDashboard parentDashboard)
        {
            InitializeComponent();
            _sellerId = sellerId;
            _dbContext = new DataContextDataContext();
            _parentDashboard = parentDashboard;

            LoadAuctions();
        }

        private void LoadAuctions()
        {
            // Fetch auctions for the current seller
            var auctions = _dbContext.Auctions
                .Where(a => a.SellerID == _sellerId)
                .Select(a => new
                {
                    a.AuctionID,
                    a.Product.ProductName,
                    a.CurrentPrice,
                    a.StartTime,
                    a.EndTime
                }).ToList();

            // Populate the DataGrid
            AuctionsDataGrid.ItemsSource = auctions;
        }


    }
}
