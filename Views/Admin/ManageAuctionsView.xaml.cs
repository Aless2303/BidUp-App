using System.Linq;
using System.Windows;
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
                        auction.AuctionID,
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
                        LastBidderName = auction.CurrentBidderID != null
                            ? dbContext.Users
                                .Where(user => user.UserID == auction.CurrentBidderID)
                                .Select(user => user.FullName)
                                .FirstOrDefault()
                            : "None",
                        // Compute Visibility directly in the data
                        CloseButtonVisibility = auction.IsClosed == true ? Visibility.Collapsed : Visibility.Visible
                    })
                    .ToList();

                AuctionListView.ItemsSource = auctions;
            }
        }

        private void CloseAuctionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int auctionId)
            {
                using (var dbContext = new DataContextDataContext())
                {
                    var auction = dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);
                    if (auction != null && (auction.IsClosed == false || auction.IsClosed == null)) // Explicitly check the nullable bool
                    {
                        auction.IsClosed = true; // Mark the auction as closed
                        dbContext.SubmitChanges(); // Save changes to the database
                        LoadAuctions(); // Refresh the UI
                        MessageBox.Show($"Auction '{auction.ProductName}' has been successfully closed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

    }
}
