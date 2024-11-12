using System.Windows;
using System.Windows.Controls;
using BidUp_App.Models.Users;
using System.Linq;

namespace BidUp_App.Views.Admin
{
    public partial class UserDetailsView : UserControl
    {
        private User _user;

        public UserDetailsView(User user)
        {
            InitializeComponent();
            _user = user;
            this.DataContext = _user;  // Set the DataContext to the passed user object
        }

        // Event handler for "Vezi Istoric" button
        private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Creăm un UserHistoryView și îl afișăm în MainContent din AdminDashboard
            var userHistoryView = new UserHistoryView(_user.UserID);
            var parentWindow = Application.Current.Windows.OfType<AdminDashboard>().FirstOrDefault();
            if (parentWindow != null)
            {
                parentWindow.MainContent.Content = userHistoryView;  // Înlocuim conținutul principal
            }
        }


        // Event handler for "Back" button
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back to the ManageUsersView
            var manageUsersView = new ManageUsersView();
            var parentWindow = Application.Current.Windows.OfType<AdminDashboard>().FirstOrDefault();
            if (parentWindow != null)
            {
                parentWindow.MainContent.Content = manageUsersView;  // Update the content in AdminDashboard
            }
        }
    }
}
