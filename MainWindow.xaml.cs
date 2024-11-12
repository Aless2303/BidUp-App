using BidUp_App.Models;
using BidUp_App.Models.Users;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace BidUp_App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;
            string role = RoleComboBox.Text;
            // Validate input fields
            if (!ValidateInputs(email, password, role))
                return;

            using (var context = new DataContextDataContext())
            {
                // Hash the password
                string passwordHash = HashPassword(password);

                // Retrieve the user from the database using LINQ
                var dbUser = context.Users.SingleOrDefault(u => u.Email == email && u.PasswordHash == passwordHash);

                if (dbUser != null)
                {
                    // Check if the role is valid
                    if (string.IsNullOrEmpty(dbUser.Role) || !IsValidRole(dbUser.Role))
                    {
                        MessageBox.Show("Invalid role assigned to this user. Please contact support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Create a User object based on the role
                    BidUp_App.Models.Users.User user = UserFactory.CreateUser(dbUser.Role);

                    // Populate the User object
                    user.m_userID = dbUser.UserID;
                    user.m_fullName = dbUser.FullName;
                    user.m_email = dbUser.Email;
                    user.m_BirthDate = dbUser.BirthDate;
                    user.ProfilePicturePath = dbUser.ProfilePicturePath;
                    user.m_password = dbUser.PasswordHash;

                    // If the user is a Bidder or Seller, load their Card information
                    if (user is Bidder || user is Seller)
                    {
                        var card = context.Cards.SingleOrDefault(c => c.OwnerUserID == dbUser.UserID);

                        if (card != null)
                        {
                            BidUp_App.Models.Card userCard = new BidUp_App.Models.Card
                            {
                                CardID = card.CardID,
                                CardNumber = card.CardNumber,
                                CardHolderName = card.CardHolderName,
                                ExpiryDate = card.ExpiryDate,
                                CVV = card.CVV,
                                Balance = (float)card.Balance
                            };

                            if (user is BidUp_App.Models.Users.Bidder bidder)
                            {
                                bidder.card = userCard;
                            }
                            else if (user is Seller seller)
                            {
                                seller.card = userCard;
                            }
                        }
                    }

                    // Display the appropriate dashboard based on the user's role
                    user.displayDasboard();

                    // Close the SignIn window
                    this.Close();
                }
                else
                {
                    // Show error if the user is not found
                    MessageBox.Show("Invalid email or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateInputs(string email, string password, string role)
        {
            // Check if email and password are not empty
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate email format
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format. Please enter a valid email.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate role selection
            if (string.IsNullOrEmpty(role) || role == "Select Role")
            {
                MessageBox.Show("Please select a valid role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Password length validation
            //if (password.Length < 6)
            //{
            //    MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return false;
            //}

            return true;
        }

        private bool IsValidRole(string role)
        {
            // Define the valid roles
            string[] validRoles = { "Admin", "Bidder", "Seller" };
            return validRoles.Contains(role);
        }

        private void Button_Click_SignUp(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
