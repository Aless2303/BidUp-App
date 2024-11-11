using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace BidUp_App.Views.Seller
{
    public partial class AddAuctionWindow : Window
    {
        private readonly DataContextDataContext _dbContext;
        private readonly int _sellerId; // ID-ul seller-ului curent

        private string _productImagePath = null; // Calea imaginii produsului

        public AddAuctionWindow(int sellerId)
        {
            InitializeComponent();
            _dbContext = new DataContextDataContext();
            _sellerId = sellerId;
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                _productImagePath = openFileDialog.FileName;
                ImagePathTextBlock.Text = _productImagePath;
            }
        }

        private void AddAuctionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validarea câmpurilor
                if (string.IsNullOrEmpty(ProductNameTextBox.Text) || string.IsNullOrEmpty(StartingPriceTextBox.Text) ||
                    !StartTimePicker.SelectedDate.HasValue || !EndTimePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string productName = ProductNameTextBox.Text;
                string description = DescriptionTextBox.Text;
                double startingPrice = double.Parse(StartingPriceTextBox.Text);
                DateTime startTime = StartTimePicker.SelectedDate.Value;
                DateTime endTime = EndTimePicker.SelectedDate.Value;

                // Validarea timpului de finalizare
                if (endTime <= startTime)
                {
                    MessageBox.Show("End Time must be after Start Time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Crearea produsului și adăugarea în baza de date
                var newProduct = new Product
                {
                    ProductName = productName,
                    Description = description,
                    ProductImagePath = _productImagePath,
                    Category = "Default", // Poți adăuga o opțiune de categorie
                    CreationDate = DateTime.Now,
                    SellerID = _sellerId
                };

                _dbContext.Products.InsertOnSubmit(newProduct);
                _dbContext.SubmitChanges();

                // Recuperarea ProductID
                int productId = newProduct.ProductID;

                // Crearea licitației și adăugarea în baza de date
                var newAuction = new Auction
                {
                    ProductID = productId,
                    ProductName = productName,
                    ProductImagePath= _productImagePath,
                    StartingPrice = startingPrice,
                    CurrentPrice = startingPrice,
                    SellerID = _sellerId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsClosed = false
                };

                _dbContext.Auctions.InsertOnSubmit(newAuction);
                _dbContext.SubmitChanges();

                MessageBox.Show("Auction added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                var linqUser = _dbContext.Users.FirstOrDefault(u => u.UserID == _sellerId);

                if (linqUser != null)
                {
                    // Convertim obiectul LINQ User într-un obiect de tip Seller
                    var sellerUser = new BidUp_App.Models.Users.Seller
                    {
                        m_userID = linqUser.UserID,
                        m_fullName = linqUser.FullName,
                        m_email = linqUser.Email,
                        m_BirthDate = linqUser.BirthDate,
                        m_password = linqUser.PasswordHash,
                        ProfilePicturePath = linqUser.ProfilePicturePath
                    };

                    // Navigăm înapoi la `SellerDashboard` cu obiectul `Seller`
                    var sellerDashboard = new SellerDashboard(sellerUser);
                    this.Close(); // Închide fereastra curentă
                    sellerDashboard.Show();
                }
                else
                {
                    MessageBox.Show("Could not retrieve seller information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            

            // Găsim utilizatorul curent din baza de date folosind ID-ul seller-ului
            var linqUser = _dbContext.Users.FirstOrDefault(u => u.UserID == _sellerId);

            if (linqUser != null)
            {
                // Convertim obiectul LINQ User într-un obiect de tip Seller
                var sellerUser = new BidUp_App.Models.Users.Seller
                {
                    m_userID = linqUser.UserID,
                    m_fullName = linqUser.FullName,
                    m_email = linqUser.Email,
                    m_BirthDate = linqUser.BirthDate,
                    m_password = linqUser.PasswordHash,
                    ProfilePicturePath = linqUser.ProfilePicturePath
                };

                // Navigăm înapoi la `SellerDashboard` cu obiectul `Seller`
                var sellerDashboard = new SellerDashboard(sellerUser);
                this.Close(); // Închide fereastra curentă
                sellerDashboard.Show();
            }
            else
            {
                MessageBox.Show("Could not retrieve seller information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
