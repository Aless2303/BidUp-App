using System.Linq;
using System.Windows.Controls;

namespace BidUp_App.Views.Admin
{
    public partial class ManageAuctionsView : UserControl
    {
        public ManageAuctionsView()
        {
            InitializeComponent();
            LoadAuctions();
        }

        private void LoadAuctions()
        {
            using (var dbContext = new DataContextDataContext())
            {
                var auctions = dbContext.Auctions
                    .Select(auction => new
                    {
                        auction.ProductName,
                        auction.Description,
                        auction.ProductImagePath,
                        auction.StartingPrice,
                        auction.CurrentPrice,
                        auction.StartTime,
                        auction.EndTime,
                        auction.IsClosed,
                        SellerName = dbContext.Users
                            .Where(user => user.UserID == auction.SellerID)
                            .Select(user => user.FullName)
                            .FirstOrDefault(),
                        LastBidderName = auction.CurrentBidderID != null ?
                            dbContext.Users
                                .Where(user => user.UserID == auction.CurrentBidderID)
                                .Select(user => user.FullName)
                                .FirstOrDefault()
                            : "Nimeni"
                    })
                    .ToList();

                AuctionListView.ItemsSource = auctions;
            }
        }
    }
}
