using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BidUp_App.Models;

namespace BidUp_App.Views.Admin
{
    public partial class ManageUsersView : UserControl
    {
        public ManageUsersView()
        {
            InitializeComponent();
            LoadUsers();
        }

        // Method to load users from the database
        private void LoadUsers()
        {
            using (var dbContext = new DataContextDataContext())
            {
                var users = dbContext.Users.Where(u => u.Role == "Seller" || u.Role == "Bidder").ToList();
                UsersListView.ItemsSource = users;
            }
        }

        private void UserDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var userId = (int)button.Tag;  // Assuming the Button Tag holds the UserId

            // Fetch the user details from the database
            using (var dbContext = new DataContextDataContext())
            {
                var user = dbContext.Users.SingleOrDefault(u => u.UserID == userId);
                if (user != null)
                {
                    // Create and navigate to UserDetailsView
                    var userDetailsView = new UserDetailsView(user);
                    var parentWindow = Application.Current.Windows.OfType<AdminDashboard>().FirstOrDefault();
                    if (parentWindow != null)
                    {
                        parentWindow.MainContent.Content = userDetailsView;  // Navigate to UserDetailsView
                    }
                }
                else
                {
                    MessageBox.Show("User not found.");
                }
            }
        }




        // Handler for the 'Șterge utilizator' button
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the UserID from the button's Tag
            var button = sender as Button;
            var userId = (int)button.Tag;

            // Delete the user with the specified UserID
            using (var dbContext = new DataContextDataContext())
            {
                var user = dbContext.Users.SingleOrDefault(u => u.UserID == userId);
                if (user != null)
                {
                    dbContext.Users.DeleteOnSubmit(user);  // Use DeleteOnSubmit instead of Remove
                    dbContext.SubmitChanges();  // Use SubmitChanges to persist the deletion
                    MessageBox.Show($"User with ID: {userId} has been deleted.");
                    LoadUsers();  // Refresh the user list after deletion
                }
                else
                {
                    MessageBox.Show("User not found.");
                }
            }
        }
    }
}
