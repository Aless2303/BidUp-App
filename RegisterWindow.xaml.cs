using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string monthName;
            int month;
            // Get Birth Date values from ComboBoxes
            if (int.TryParse(BirthDayComboBox.SelectedItem?.ToString(), out int day) &&
                int.TryParse(BirthYearComboBox.SelectedItem?.ToString(), out int year) &&
                BirthMonthComboBox.SelectedItem != null)
            {
                monthName = BirthMonthComboBox.SelectedItem.ToString();
                month = DateTime.ParseExact(monthName, "MMMM", null).Month; // Convert month name to month number
                try
                {
                    DateTime birthDate = new DateTime(year, month, day);
                    // Continue processing birthDate
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("Invalid birth date selected. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please select a valid birth date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Card information
            string cardNumber = CardNumberTextBox.Text;
            string cardHolderName = CardHolderNameTextBox.Text;
            string expiryDate = ExpiryDateTextBox.Text;
            string cvv = CVVTextBox.Text;
            decimal balance = 0;

            // Validate inputs
            if (!ValidateInputs(fullName, email, password, confirmPassword, cardNumber, cardHolderName, expiryDate, cvv))
                return;

            // Hash the password
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
                        BirthDate = new DateTime(year, month, day),
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
                            ExpiryDate = DateTime.ParseExact(expiryDate, "MM/yy", null),
                            CVV = cvv,
                            Balance = balance,
                            OwnerUserID = newUser.UserID // Link the card to the newly created user
                        };

                        context.Cards.InsertOnSubmit(newCard);
                        context.SubmitChanges();
                    }

                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs(string fullName, string email, string password, string confirmPassword, string cardNumber, string cardHolderName, string expiryDate, string cvv)
        {
            // Check if fields are empty
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(cardHolderName) ||
                string.IsNullOrEmpty(expiryDate) || string.IsNullOrEmpty(cvv))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Email validation
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Password match validation
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Card number validation (13 digits)
            if (!Regex.IsMatch(cardNumber, @"^\d{13}$"))
            {
                MessageBox.Show("Card number must be exactly 13 digits.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // CVV validation
            if (!Regex.IsMatch(cvv, @"^\d{1,3}$"))
            {
                MessageBox.Show("CVV must be up to 3 digits.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Expiry date format validation
            if (!Regex.IsMatch(expiryDate, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                MessageBox.Show("Expiry date must be in MM/YY format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
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
