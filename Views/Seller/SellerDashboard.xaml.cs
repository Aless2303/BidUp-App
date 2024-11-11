using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BidUp_App.Models.Users;

namespace BidUp_App.Views.Seller
{
    public partial class SellerDashboard : Window
    {
        private readonly BidUp_App.Models.Users.User _user;
        private readonly DataContextDataContext _dbContext;
        private readonly int _sellerID;

        public SellerDashboard(BidUp_App.Models.Users.User user)
        {
            InitializeComponent();
            _user = user;
            _dbContext = new DataContextDataContext();
            if (_user is BidUp_App.Models.Users.Seller seller)
            {
                _sellerID = seller.m_userID; // Obține ID-ul utilizatorului curent
            }
            LoadProfile();
        }

        private void LoadProfile()
        {
            // Încarcă poza de profil dacă există
            if (!string.IsNullOrEmpty(_user.ProfilePicturePath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_user.ProfilePicturePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ProfileImage.Source = bitmap;
            }

            // Afișează detaliile utilizatorului
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
        }

        private void AddNewAuctionButton_Click(object sender, RoutedEventArgs e)
        {
            
            var addAuctionWindow = new AddAuctionWindow(_sellerID);
            this.Close();// Instanțiere fereastră `AddAuctionWindow` cu ID-ul sellerului
            addAuctionWindow.Show(); // Afișează fereastra
        }


        private void ViewYourAuctionsButton_Click(object sender, RoutedEventArgs e)
        {
            var viewAuctionsWindow = new ViewAuctionsWindow(_user.m_userID); // Transmite ID-ul Seller-ului curent
            viewAuctionsWindow.ShowDialog();
            this.Close();
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
