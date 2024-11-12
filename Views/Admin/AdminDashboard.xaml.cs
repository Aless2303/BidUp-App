using System.Windows;
using BidUp_App.Views.Admin;

namespace BidUp_App.Views.Admin
{
    public partial class AdminDashboard : Window
    {
        BidUp_App.Models.Users.User user;
        public AdminDashboard(BidUp_App.Models.Users.User user)
        {
            InitializeComponent();
            this.user = user;
            // Load default content (Profile view)
            MainContent.Content = new AdminProfileView(user);  // Încărcăm AdminProfileView la început
        }

        // Handler pentru butonul "Profile"
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Încarcă profilul utilizatorului
            MainContent.Content = new AdminProfileView(user);  // Încarcă AdminProfileView
        }

        // Handler pentru butonul "Manage Users"
        private void ManageUsersButton_Click(object sender, RoutedEventArgs e)
        {
            // Încarcă secțiunea pentru gestionarea utilizatorilor
            MainContent.Content = new ManageUsersView();  // Încarcă ManageUsersView
        }

        // Handler pentru butonul "Manage Auctions"
        private void ManageAuctionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Încarcă secțiunea pentru gestionarea licitațiilor
            MainContent.Content = new ManageAuctionsView();  // Încarcă ManageAuctionsView
        }
    }
}
