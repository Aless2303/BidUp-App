using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BidUp_App.Models.Users;
using BidUp_App.Views.Seller;

namespace BidUp_App.Views.Bidder
{
    public partial class BidderDashboard : Window
    {
        private readonly BidUp_App.Models.Users.User _user;
        private readonly DataContextDataContext _dbContext;

        public BidderDashboard(BidUp_App.Models.Users.User user)
        {
            InitializeComponent();
            _user = user;
            _dbContext = new DataContextDataContext(); // Instanțiere DataContext
            LoadProfile();
        }

        private void LoadProfile()
        {
            if (!string.IsNullOrEmpty(_user.ProfilePicturePath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_user.ProfilePicturePath, UriKind.Absolute);
                bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ProfileImage.Source = bitmap;
            }

            FullNameTextBlock.Text = _user.m_fullName;
            EmailTextBlock.Text = _user.m_email;
            DateOfBirthTextBlock.Text = _user.m_BirthDate.ToString("d");
            RoleTextBlock.Text = _user.m_role;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayProfileInfo();
        }

        private void DisplayProfileInfo()
        {
            FullNameTextBlock.Visibility = Visibility.Visible;
            EmailTextBlock.Visibility = Visibility.Visible;
            DateOfBirthTextBlock.Visibility = Visibility.Visible;
            RoleTextBlock.Visibility = Visibility.Visible;
            CardInfoPanel.Visibility = Visibility.Hidden; // Ascunde informațiile despre card
        }

        private void SeeNewAuctionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Instanțiază fereastra pentru vizualizarea licitațiilor
            var viewAuctionsWindow = new ViewAuctionsWindowBidder(_user.m_userID);

            // Afișează fereastra
            viewAuctionsWindow.Show();
        }

        private void SeeLastBidsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navigating to your last bids...");
        }

        private void ViewCardButton_Click(object sender, RoutedEventArgs e)
        {
            string password = PromptForPassword();

            if (IsPasswordValid(password))
            {
                ShowCardInfo();
            }
            else
            {
                MessageBox.Show("Incorrect password. Please try again.", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string PromptForPassword()
        {
            return Microsoft.VisualBasic.Interaction.InputBox("Enter your password to view card details:", "Password Required", "");
        }

        private bool IsPasswordValid(string enteredPassword)
        {
            string enteredPasswordHash = HashPassword(enteredPassword);
            return enteredPasswordHash == _user.m_password;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void ShowCardInfo()
        {
            if (_user is BidUp_App.Models.Users.Bidder  bidder)
            {
                var card = _dbContext.Cards.FirstOrDefault(c => c.OwnerUserID == bidder.m_userID);

                if (card != null)
                {
                    CardInfoPanel.Visibility = Visibility.Visible;
                    CardHolderTextBlock.Text = card.CardHolderName;
                    CardNumberTextBlock.Text = "**** **** **** " + card.CardNumber.Substring(card.CardNumber.Length - 4);
                    ExpiryDateTextBlock.Text = card.ExpiryDate.ToString("MM/yy");
                }
                else
                {
                    MessageBox.Show("No card information found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ChangeProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                ProfileImage.Source = new BitmapImage(new Uri(selectedFilePath));
                _user.ProfilePicturePath = selectedFilePath;
                SaveProfilePicturePathToDatabase(_user.m_userID, selectedFilePath);
            }
        }

        private void SaveProfilePicturePathToDatabase(int userId, string profilePicturePath)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
            if (user != null)
            {
                user.ProfilePicturePath = profilePicturePath;
                _dbContext.SubmitChanges();
            }
        }
    }
}
