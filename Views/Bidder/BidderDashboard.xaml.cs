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
            LoadProfileView(); // Default View
            InitializeWalletUpdateTimer();
        }

        private void LoadProfileView()
        {
            var profileView = new ProfileView(_user, _dbContext); // Pass user and dbContext
            ContentViewbox.Child = profileView;
        }

        private void LoadWalletBalance()
        {
            _dbContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _dbContext.Wallets);
            var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _user.m_userID);
            WalletBalanceText.Text = wallet != null ? $"{wallet.Balance:C}" : "$0.00";
        }

        private void InitializeWalletUpdateTimer()
        {
            _walletUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Interval de verificare la fiecare secundă
            };
            _walletUpdateTimer.Tick += WalletUpdateTimer_Tick;
            _walletUpdateTimer.Start();
        }

        private void WalletUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Obține soldul curent din baza de date
                _dbContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _dbContext.Wallets);
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _user.m_userID);

                if (wallet != null)
                {
                    // Obține soldul afișat în prezent
                    if (decimal.TryParse(WalletBalanceText.Text, System.Globalization.NumberStyles.Currency,
                        System.Globalization.CultureInfo.CurrentCulture, out var currentDisplayedBalance))
                    {
                        // Compară valoarea afișată cu valoarea din baza de date
                        if (wallet.Balance != currentDisplayedBalance)
                        {
                            // Dacă este diferită, actualizează UI-ul
                            WalletBalanceText.Text = $"{wallet.Balance:C}";
                        }
                    }
                    else
                    {
                        // Dacă parsarea eșuează, actualizează direct cu valoarea din baza de date
                        WalletBalanceText.Text = $"{wallet.Balance:C}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating wallet balance: {ex.Message}");
            }
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
            // Încarcă `LastBidsView` în `ContentViewbox`
            var lastBidsView = new LastBids(_user.m_userID);
            ContentViewbox.Child = lastBidsView;
        }


    }
}
