using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace BidUp_App.Views.Seller
{
    public partial class AddAuctionControl : UserControl
    {
        private readonly DataContextDataContext _dbContext;
        private readonly int _sellerId; // Current seller's ID
        private string _productImagePath = null; // Path to product image

        public AddAuctionControl(int sellerId)
        {
            InitializeComponent();
            _dbContext = new DataContextDataContext();
            _sellerId = sellerId;
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

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
                // Validate fields
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

                if (endTime <= startTime)
                {
                    MessageBox.Show("End Time must be after Start Time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create product and add to database
                var newProduct = new Product
                {
                    ProductName = productName,
                    Description = description,
                    ProductImagePath = _productImagePath,
                    Category = "Default",
                    CreationDate = DateTime.Now,
                    SellerID = _sellerId
                };

                _dbContext.Products.InsertOnSubmit(newProduct);
                _dbContext.SubmitChanges();

                int productId = newProduct.ProductID;

                // Create auction and add to database
                var newAuction = new Auction
                {
                    ProductID = productId,
                    ProductName = productName,
                    ProductImagePath = _productImagePath,
                    StartingPrice = startingPrice,
                    CurrentPrice = startingPrice,
                    SellerID = _sellerId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsClosed = false
                };

                _dbContext.Auctions.InsertOnSubmit(newAuction);
                _dbContext.SubmitChanges();

                int auctionId = newAuction.AuctionID;

                // Create log for this event
                var sellerName = _dbContext.Users.FirstOrDefault(u => u.UserID == _sellerId)?.FullName;
                var message = $"Seller {sellerName} added {productName} to the auction starting at {startingPrice:C}.";

                var log = new Log
                {
                    Timestamp = DateTime.Now,
                    EventType = "AddAuction",
                    Message = message,
                    DynamicData = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        SellerID = _sellerId,
                        AuctionID = auctionId,
                        StartPrice = startingPrice
                    })
                };

                _dbContext.Logs.InsertOnSubmit(log);
                _dbContext.SubmitChanges();

                MessageBox.Show("Auction added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Auction creation canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
