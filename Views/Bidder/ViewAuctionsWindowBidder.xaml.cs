using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BidUp_App.Models;

namespace BidUp_App.Views.Bidder
{
    public partial class ViewAuctionsControl : UserControl
    {
        private readonly DataContextDataContext _dbContext;
        private readonly int _currentBidderId;
        private DispatcherTimer _timer;
        private DispatcherTimer _notificationTimer;

        public ViewAuctionsControl(int currentBidderId)
        {
            InitializeComponent();
            _dbContext = new DataContextDataContext();
            _currentBidderId = currentBidderId;

            LoadAuctions();
            CheckNotifications();

            InitializeTimers();
        }

        private void InitializeTimers()
        {
            // Timer for updating auction time remaining
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Timer for checking notifications
            _notificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3) // Check notifications every 3 seconds
            };
            _notificationTimer.Tick += NotificationTimer_Tick;
            _notificationTimer.Start();
        }

        private void LoadAuctions()
        {
            _dbContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _dbContext.Auctions);

            var auctions = _dbContext.Auctions
                .Where(a => a.EndTime > DateTime.Now) // Only active auctions
                .AsEnumerable() // Execute SQL and bring data into memory
                .Select(a => new BidUp_App.Models.Users.AuctionViewModel
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
                        ? $"Start in: {GetRemainingTime(a.StartTime)}"
                        : $"Time Left: {GetRemainingTime(a.EndTime)}" // Calculate RemainingTime locally
                })
                .ToList();

            AuctionsList.ItemsSource = null; // Reset to force refresh
            AuctionsList.ItemsSource = auctions;
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var item in AuctionsList.Items)
            {
                if (item is BidUp_App.Models.Users.AuctionViewModel auction)
                {
                    if (auction.StartTime > DateTime.Now)
                    {
                        auction.RemainingTime = $"Start in: {GetRemainingTime(auction.StartTime)}";
                    }
                    else
                    {
                        auction.RemainingTime = $"Time Left: {GetRemainingTime(auction.EndTime)}";
                    }
                }
            }

            AuctionsList.Items.Refresh();
        }

        private string GetRemainingTime(DateTime time)
        {
            var remainingTime = time - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                return "Expired";
            }

            return $"{remainingTime.Days}d {remainingTime.Hours}h {remainingTime.Minutes}m {remainingTime.Seconds}s";
        }

        private void BidButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int auctionId)
            {
                var selectedAuction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);

                if (selectedAuction != null)
                {
                    if (selectedAuction.StartTime > DateTime.Now)
                    {
                        MessageBox.Show("This auction has not started yet. Please check back later.", "Auction Not Started", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Open the BidWindow (modal)
                    var bidWindow = new BidWindow(selectedAuction, _currentBidderId);
                    if (bidWindow.ShowDialog() == true)
                    {
                        LoadAuctions();
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
            var notifications = _dbContext.Notifications
                .Where(n => n.BidderID == _currentBidderId && !n.IsRead)
                .ToList();

            foreach (var notification in notifications)
            {
                MessageBox.Show(notification.Message, "New Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                notification.IsRead = true;
            }

            if (notifications.Any())
            {
                _dbContext.SubmitChanges();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            LoadAuctions();
            _timer.Start();
        }
    }

}
