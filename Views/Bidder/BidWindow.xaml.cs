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
                if (string.IsNullOrEmpty(BidAmountTextBox.Text) || !decimal.TryParse(BidAmountTextBox.Text, out decimal bidAmount))
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
                if (bidAmount <= (decimal)auction.CurrentPrice)
                {
                    MessageBox.Show("The bid amount must be higher than the current price.", "Invalid Bid", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Obține portofelul utilizatorului curent
                var currentBidderWallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _currentBidderId);
                if (currentBidderWallet == null || currentBidderWallet.Balance < bidAmount)
                {
                    MessageBox.Show("You do not have enough funds in your wallet to place this bid.", "Insufficient Funds", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifică dacă există un licitant anterior
                if (auction.CurrentBidderID != null)
                {
                    int previousBidderId = auction.CurrentBidderID.Value;

                    // Obține portofelul licitantului anterior
                    var previousBidderWallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == previousBidderId);
                    if (previousBidderWallet != null)
                    {
                        // Returnează banii licitantului anterior
                        previousBidderWallet.Balance += (decimal)auction.CurrentPrice;

                        //Adaugă notificare pentru licitantul anterior
                       var notification = new Notification
                       {
                           BidderID = previousBidderId,
                           AuctionID = auction.AuctionID,
                           Message = $"Your previous bid of {auction.CurrentPrice:C} was outbid for {auction.ProductName}.",
                           CreatedAt = DateTime.Now,
                           IsRead = false
                       };
                        _dbContext.Notifications.InsertOnSubmit(notification);
                    }
                }

                // Scade fondurile din portofelul utilizatorului curent
                currentBidderWallet.Balance -= bidAmount;

                // Actualizează datele licitației
                auction.CurrentPrice = (double)bidAmount;
                auction.CurrentBidderID = _currentBidderId;


                // Salvează modificările în baza de date
                _dbContext.SubmitChanges();

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