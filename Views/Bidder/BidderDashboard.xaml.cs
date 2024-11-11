using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BidUp_App.Models;
using BidUp_App.Models.Users;

namespace BidUp_App.Views.Bidder
{
    public partial class BidderDashboard : Window
    {
        private readonly BidUp_App.Models.Users.User _user;
        private readonly DataContextDataContext _dbContext;
        private DispatcherTimer _walletUpdateTimer;

        public BidderDashboard(BidUp_App.Models.Users.User user)
        {
            InitializeComponent();
            _user = user;
            _dbContext = new DataContextDataContext();

            LoadWalletBalance();
            InitializeWalletUpdateTimer();
            LoadProfileView(); // Default View
        }

        private void LoadProfileView()
        {
            var profileView = new ProfileView(_user, _dbContext); // Pass user and dbContext
            ContentViewbox.Child = profileView;
        }

        private void LoadWalletBalance()
        {
            var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _user.m_userID);
            WalletBalanceText.Text = wallet != null ? $"{wallet.Balance:C}" : "$0.00";
        }

        private void InitializeWalletUpdateTimer()
        {
            _walletUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _walletUpdateTimer.Tick += (s, e) =>
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _user.m_userID);
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
            LoadProfileView();
        }

        private void SeeNewAuctionsButton_Click(object sender, RoutedEventArgs e)
        {
            var auctionsView = new ViewAuctionsControl(_user.m_userID);
            ContentViewbox.Child = auctionsView;
        }


        private void SeeLastBidsButton_Click(object sender, RoutedEventArgs e)
        {
            // Load a different UserControl for last bids (if implemented)
            MessageBox.Show("Load Last Bids View...");
        }


    }
}
