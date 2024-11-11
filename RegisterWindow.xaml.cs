using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BidUp_App
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Toggle the visibility of the password placeholder based on the PasswordBox content
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }


        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Toggle the visibility of the confirm password placeholder based on the PasswordBox content
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the input values from the fields
            string fullName = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            DateTime? birthDate = DateOfBirthPicker.SelectedDate;
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Card information
            string cardNumber = CardNumberTextBox.Text;
            string cardHolderName = CardHolderNameTextBox.Text;
            DateTime? expiryDate = ExpiryDatePicker.SelectedDate;
            string cvv = CVVTextBox.Text;
            decimal balance = 0;

            // Validate fields
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword) || !birthDate.HasValue || role == "Select Role" ||
                string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(cardHolderName) || !expiryDate.HasValue || string.IsNullOrEmpty(cvv))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if passwords match
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Hash the password (you may replace this with your own hash function)
            string passwordHash = HashPassword(password);
            string defaultProfilePicturePath = @"C:\Users\Florea\source\repos\BidUp-App\Resources\profil2.png";

            try
            {
                // Use LINQ to SQL DataContext
                using (var context = new DataContextDataContext()) // Replace with your DataContext class name
                {
                    // Create and insert the new User
                    var newUser = new User
                    {
                        FullName = fullName,
                        PasswordHash = passwordHash,
                        Role = role,
                        Email = email,
                        BirthDate = birthDate.Value,
                        ProfilePicturePath = defaultProfilePicturePath,
                        CreatedAt = DateTime.Now
                    };

                    context.Users.InsertOnSubmit(newUser);
                    context.SubmitChanges();

                    // If the role is Bidder or Seller, create a card for the user
                    if (role == "Bidder" || role == "Seller")
                    {
                        var newCard = new Card
                        {
                            CardNumber = cardNumber,
                            CardHolderName = cardHolderName,
                            ExpiryDate = expiryDate.Value,
                            CVV = cvv,
                            Balance = balance,
                            OwnerUserID = newUser.UserID // Link the card to the newly created user
                        };

                        context.Cards.InsertOnSubmit(newCard);
                        context.SubmitChanges();
                    }

                    // Success message
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Handle errors gracefully
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var byteValue in bytes)
                {
                    builder.Append(byteValue.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void BackToSignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the MainWindow
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
