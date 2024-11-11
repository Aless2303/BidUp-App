using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using BidUp_App.Models;
using BidUp_App.Models.Users;

namespace BidUp_App.Views.Bidder
{
    public partial class ViewAuctionsWindowBidder : Window
    {
        private readonly DataContextDataContext _dbContext;
        private readonly int _currentBidderId;
        private DispatcherTimer _timer;
        private DispatcherTimer _notificationTimer; // Timer pentru notificări


        public ViewAuctionsWindowBidder(int currentBidderId)
        {
            InitializeComponent();
            _dbContext = new DataContextDataContext();
            _currentBidderId = currentBidderId;
            LoadAuctions();
            UpdateWalletBalance();
            CheckNotifications();

            // Inițializează și pornește timerul
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            _notificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3) // Verifică notificările la fiecare 3 secunde
            };
            _notificationTimer.Tick += NotificationTimer_Tick;
            _notificationTimer.Start();

        }

        private void LoadAuctions()
        {

            _dbContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _dbContext.Auctions);

            var auctions = _dbContext.Auctions
                .Where(a => a.EndTime > DateTime.Now) // Doar licitații active
                .AsEnumerable() // Execută interogarea SQL și aduce datele în memorie
                .Select(a => new AuctionViewModel
                {
                    AuctionID = a.AuctionID,
                    ProductName = a.ProductName,
                    Description = a.Description,
                    StartingPrice = a.StartingPrice,
                    CurrentPrice = a.CurrentPrice,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    ProductImagePath = a.ProductImagePath,
                    RemainingTime = a.StartTime > DateTime.Now
                        ? "Not Started"
                        : GetRemainingTime(a.EndTime) // Calculează RemainingTime local
                })
                .ToList();

            // Actualizează sursa de date a ItemsControl
            AuctionsList.ItemsSource = null; // Resetare pentru a forța reîmprospătarea
            AuctionsList.ItemsSource = auctions;
        }

        private void UpdateWalletBalance()
        {
            _dbContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _dbContext.Wallets);
            var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _currentBidderId);
            WalletBalanceText.Text = wallet != null ? $"{wallet.Balance:C}" : "$0.00";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var item in AuctionsList.Items)
            {
                if (item is AuctionViewModel auction)
                {
                    // Nu actualizăm timpul pentru licitațiile care nu au început
                    if (auction.StartTime > DateTime.Now)
                    {
                        auction.RemainingTime = "Not Started";
                    }
                    else
                    {
                        auction.RemainingTime = GetRemainingTime(auction.EndTime);
                    }
                }
            }

            // Reînnoiește ItemsControl-ul
            AuctionsList.Items.Refresh();
        }

        private string GetRemainingTime(DateTime endTime)
        {
            var remainingTime = endTime - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                return "Expired";
            }

            return $"{remainingTime.Days}d {remainingTime.Hours}h {remainingTime.Minutes}m {remainingTime.Seconds}s";
        }

        private void BidButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                int auctionId = (int)button.CommandParameter;

                // Găsește licitația selectată
                var selectedAuction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);

                if (selectedAuction != null)
                {
                    if (selectedAuction.StartTime > DateTime.Now)
                    {
                        // Licitația nu a început încă
                        MessageBox.Show("This auction has not started yet. Please check back later.", "Auction Not Started", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Deschide fereastra `BidWindow`
                    var bidWindow = new BidWindow(selectedAuction, _currentBidderId);

                    // Așteaptă finalizarea ferestrei `BidWindow`
                    if (bidWindow.ShowDialog() == true) // Se deschide modal
                    {
                        // Reîncarcă lista de licitații după plasarea unui bid
                        LoadAuctions();
                        UpdateWalletBalance();
                    }
                }
                else
                {
                    MessageBox.Show("Auction not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void NotificationTimer_Tick(object sender, EventArgs e)
        {
            CheckNotifications();
        }

        private void CheckNotifications()
        {
            // Găsește notificările necitite pentru utilizatorul curent
            var notifications = _dbContext.Notifications
                .Where(n => n.BidderID == _currentBidderId && !n.IsRead)
                .ToList();

            // Afișează notificările
            foreach (var notification in notifications)
            {
                MessageBox.Show(notification.Message, "New Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                // Marchează notificarea ca citită
                notification.IsRead = true;
            }

            // Salvează modificările în baza de date
            if (notifications.Any())
            {
                _dbContext.SubmitChanges();
            }
        }


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Oprește temporar timerul pentru a preveni conflicte
            _timer.Stop();

            // Reîncarcă lista de licitații
            LoadAuctions();

            // Reîncarcă soldul portofelului
            UpdateWalletBalance();

            // Repornim timerul după reîmprospătare
            _timer.Start();

        }



    }



}
