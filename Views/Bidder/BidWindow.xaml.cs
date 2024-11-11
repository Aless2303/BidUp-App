using System;
using System.Linq;
using System.Windows;
using BidUp_App.Models;

namespace BidUp_App.Views.Bidder
{
    public partial class BidWindow : Window
    {
        private readonly Auction _selectedAuction;
        private readonly int _currentBidderId;
        private readonly DataContextDataContext _dbContext;

        public BidWindow(Auction selectedAuction, int currentBidderId)
        {
            InitializeComponent();
            _selectedAuction = selectedAuction;
            _currentBidderId = currentBidderId;
            _dbContext = new DataContextDataContext();
        }

        private void PlaceBidButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verifică dacă valoarea introdusă este validă
                if (string.IsNullOrEmpty(BidAmountTextBox.Text) || !double.TryParse(BidAmountTextBox.Text, out double bidAmount))
                {
                    MessageBox.Show("Please enter a valid bid amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifică dacă licitația curentă există în baza de date
                var auction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == _selectedAuction.AuctionID);
                if (auction == null)
                {
                    MessageBox.Show("The selected auction does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifică dacă valoarea licitată este mai mare decât prețul curent
                if (bidAmount <= auction.CurrentPrice)
                {
                    MessageBox.Show("The bid amount must be higher than the current price.", "Invalid Bid", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Actualizează prețul curent și licitantul curent în baza de date
                auction.CurrentPrice = bidAmount;
                auction.CurrentBidderID = _currentBidderId;

                // Salvează modificările
                _dbContext.SubmitChanges();


                if (auction.CurrentBidderID != null && auction.CurrentBidderID!= _currentBidderId)
                {
                    // Salvează `CurrentBidderID` anterior pentru notificare
                    int previousBidderId = (int)auction.CurrentBidderID;

                    // Salvează notificarea pentru `previousBidderId`
                    if (previousBidderId > 0) // Dacă există un licitant anterior
                    {
                        var notification = new Notification
                        {
                            BidderID = previousBidderId,
                            AuctionID = auction.AuctionID,
                            Message = $"Another user has placed a higher bid of {bidAmount:C} for {auction.ProductName}.",
                            CreatedAt = DateTime.Now
                        };
                        _dbContext.Notifications.InsertOnSubmit(notification);
                        _dbContext.SubmitChanges();
                    }
                }

                MessageBox.Show("Your bid has been placed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Închide fereastra după plasarea licitației
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Închide fereastra fără a face modificări
            this.Close();
        }
    }
}