using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BidUp_App.Models.Users;
using BidUp_App.Views.Bidder;

namespace BidUp_App.Views.Seller
{
    public partial class SellerDashboard : Window
    {
        private readonly BidUp_App.Models.Users.User _user;
        private readonly DataContextDataContext _dbContext;
        private readonly int _sellerID;
        private DispatcherTimer _walletUpdateTimer;

        public SellerDashboard(BidUp_App.Models.Users.User user)
        {
            InitializeComponent();
            _user = user;
            _dbContext = new DataContextDataContext();
            if (_user is BidUp_App.Models.Users.Seller seller)
            {
                _sellerID = seller.m_userID; // Get current user's ID
            }

            LoadWalletBalance();
            InitializeWalletUpdateTimer();
            LoadProfileView(); // Default View on Dashboard load
        }

        private void LoadProfileView()
        {
            // Reuse the ProfileView from the Bidder section
            var profileView = new ProfileView(_user, _dbContext);
            MainContentViewbox.Child = profileView; // Load into the main content area
        }

        private void AddNewAuctionView()
        {
            var addAuctionControl = new AddAuctionControl(_sellerID);
            MainContentViewbox.Child = addAuctionControl; // Load AddAuctionControl
        }

        private void LoadWalletBalance()
        {
            // Fetch wallet balance for the current seller
            var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _sellerID);
            WalletBalanceText.Text = wallet != null ? $"{wallet.Balance:C}" : "$0.00";
        }

        private void InitializeWalletUpdateTimer()
        {
            _walletUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Update every second
            };
            _walletUpdateTimer.Tick += (s, e) =>
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _sellerID);
                if (wallet != null)
                {
                    decimal.TryParse(WalletBalanceText.Text, System.Globalization.NumberStyles.Currency,
                        System.Globalization.CultureInfo.CurrentCulture, out var displayedBalance);

                    if (wallet.Balance != displayedBalance)
                        WalletBalanceText.Text = $"{wallet.Balance:C}";
                }
            };
            _walletUpdateTimer.Start();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProfileView(); // Load ProfileView
        }

        private void AddNewAuctionButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewAuctionView(); // Load AddAuctionControl
        }

        private void ViewYourAuctionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Load ViewAuctionsControl
            var viewAuctionsControl = new ViewAuctionsControl(_sellerID, this);
            MainContentViewbox.Child = viewAuctionsControl;
        }
    }
}
