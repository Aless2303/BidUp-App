using BidUp_App.Models;
using BidUp_App.Models.Users;
using System;
using System.Linq;
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

            using (var context = new DataContextDataContext())
            {
                // Hash the password
                string passwordHash = HashPassword(password);

                // Retrieve the user from the database using LINQ
                var dbUser = context.Users.SingleOrDefault(u => u.Email == email && u.PasswordHash == passwordHash);

                if (dbUser != null)
                {
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
