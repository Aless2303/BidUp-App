using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BidUp_App.Models;
using Microsoft.Win32;

namespace BidUp_App.Views.Bidder
{
    public partial class ProfileView : UserControl
    {
        private readonly BidUp_App.Models.Users.User _user;
        private readonly DataContextDataContext _dbContext;

        public ProfileView(BidUp_App.Models.Users.User user, DataContextDataContext dbContext)
        {
            InitializeComponent();
            _user = user;
            _dbContext = dbContext;
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




        private void ViewCardButton_Click(object sender, RoutedEventArgs e)
        {
            string password = PromptForPassword();

            if (IsPasswordValid(password))
            {
                    ShowCardInfo();
                    ViewCardButton.Visibility = Visibility.Collapsed;
                    HideCardButton.Visibility = Visibility.Visible;
                
            }
            else
            {
                MessageBox.Show("Incorrect password. Please try again.", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HideCardButton_Click(object sender, RoutedEventArgs e)
        {
            // Ascunde panoul cu detaliile cardului și secțiunile pentru gestionarea fondurilor
            CardInfoPanel.Visibility = Visibility.Hidden;
            AddFundsPanel.Visibility = Visibility.Collapsed;
            DeductFundsPanel.Visibility = Visibility.Collapsed;

            // Modifică vizibilitatea butoanelor
            ViewCardButton.Visibility = Visibility.Visible;
            HideCardButton.Visibility = Visibility.Collapsed;
        }

        private void ShowCardInfo()
        {
            var card = _dbContext.Cards.FirstOrDefault(c => c.OwnerUserID == _user.m_userID);

            if (card != null)
            {
                // Actualizează textul detaliilor cardului
                CardHolderTextBlock.Text = card.CardHolderName;
                CardNumberTextBlock.Text = "**** **** **** " + card.CardNumber.Substring(card.CardNumber.Length - 4);
                ExpiryDateTextBlock.Text = card.ExpiryDate.ToString("MM/yy");

                // Afișează panoul cu detaliile cardului și butoanele de gestionare a fondurilor
                CardInfoPanel.Visibility = Visibility.Visible;
                AddFundsPanel.Visibility = Visibility.Visible;
                DeductFundsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("No card information found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void AddFundsButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AddFundsTextBox.Text, out var amount) && amount > 0)
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _user.m_userID);

                if (wallet != null)
                {
                    wallet.Balance += amount;
                    wallet.LastUpdated = DateTime.Now;
                }
                else
                {
                    wallet = new Wallet
                    {
                        UserID = _user.m_userID,
                        Balance = amount,
                        LastUpdated = DateTime.Now
                    };
                    _dbContext.Wallets.InsertOnSubmit(wallet);
                }

                _dbContext.SubmitChanges();
                MessageBox.Show("Funds added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please enter a valid amount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            AddFundsTextBox.Text = string.Empty;
        }

        private void DeductFundsButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(DeductFundsTextBox.Text, out var amount) && amount > 0)
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _user.m_userID);

                if (wallet != null && wallet.Balance >= amount)
                {
                    wallet.Balance -= amount;
                    wallet.LastUpdated = DateTime.Now;
                    _dbContext.SubmitChanges();
                    MessageBox.Show("Funds deducted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Insufficient funds in wallet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid amount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DeductFundsTextBox.Text = string.Empty;
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
